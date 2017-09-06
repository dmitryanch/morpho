﻿using System;
using System.Collections.Generic;
using System.Linq;

namespace Utils.HAMT
{
	public class Hamt<TKey, TValue>
	{
		private const int shiftAmount = 5;
		private const int maxChildrenPerNode = (1 << shiftAmount);
		private const int freeTableCount = maxChildrenPerNode + 1;
		private const int freeTableSize = 32;
		private const int maskAmount = maxChildrenPerNode - 1;
		private const int emptyArrayStore = -1;
		private const int startingLevelCount = (32 + shiftAmount - 1) / shiftAmount;

		private readonly int[] parallelArrayIndicies = new int[freeTableCount];
		private readonly INode[][] nodeArrayStore = new INode[freeTableCount * freeTableSize][];
		private Func<TKey, int> hashEvaluate = s => s.GetHashCode();
		public int ItemCount { get; private set; }

		private LinkerNode root;

		private IEqualityComparer<TKey> comparer;

		public Hamt() : this(null) { }

		public Hamt(IEqualityComparer<TKey> comparer)
		{
			Clear();

			this.comparer = comparer ?? EqualityComparer<TKey>.Default;
		}

		public void SetHashEvaluator(Func<TKey, int> rhf)
		{
			hashEvaluate = rhf;
		}

		public void Clear()
		{
			root = new LinkerNode(maxChildrenPerNode, startingLevelCount);

			ItemCount = 0;

			for (int i = 0; i < freeTableCount; i++)
			{
				parallelArrayIndicies[i] = emptyArrayStore;
			}
		}

		public void Add(TKey key, TValue value)
		{
			int rootShiftAmount = (shiftAmount * (startingLevelCount + 1 - root.Bitmap));
			int rootCount = (1 << rootShiftAmount);

			int newKeyBitGroup = this.hashEvaluate(key);
			int newKeyBitIndex = newKeyBitGroup & (rootCount - 1);

			KeyValueNode newKeyValuePair = new KeyValueNode(key, value);

			var node = root.Nodes[newKeyBitIndex];

			LinkerNode linkerNode = node as LinkerNode;

			if (linkerNode != null)
			{
				newKeyBitGroup >>= rootShiftAmount;

				for (int cLevel = 1; cLevel <= root.Bitmap; newKeyBitGroup >>= shiftAmount, cLevel++)
				{
					newKeyBitIndex = newKeyBitGroup & maskAmount;

					int indexGivenBitIndex = GetElementCountBeforeBitIndex(newKeyBitIndex, linkerNode.Bitmap);

					if (IsBitSet(newKeyBitIndex, linkerNode.Bitmap)) // Bitmap of -1 is probabilistically low with root resizing
					{
						KeyValueNode nextNodeKey = linkerNode.Nodes[indexGivenBitIndex] as KeyValueNode;

						if (nextNodeKey != null)
						{
							newKeyBitGroup >>= shiftAmount;

							ReplaceKeyWithLinkingNode(linkerNode.Nodes, indexGivenBitIndex, nextNodeKey, newKeyValuePair, newKeyBitGroup, newKeyBitIndex, cLevel);

							return;
						}
						else
						{
							linkerNode = (LinkerNode)linkerNode.Nodes[indexGivenBitIndex];
						}
					}
					else
					{
						InsertIntoLinkerNode(linkerNode, newKeyValuePair, indexGivenBitIndex, newKeyBitIndex);

						return;
					}
				}

				AppendLeavesBucket(linkerNode, newKeyValuePair);

				return;
			}

			KeyValueNode keyValueNode = node as KeyValueNode;

			if (keyValueNode != null)
			{
				newKeyBitGroup >>= rootShiftAmount;

				ReplaceKeyWithLinkingNode(root.Nodes, newKeyBitIndex, keyValueNode, newKeyValuePair, newKeyBitGroup, newKeyBitIndex, 0);
			}
			else
			{
				root.Nodes[newKeyBitIndex] = newKeyValuePair;

				ItemCount++;
			}
		}

		private void StoreArray(INode[] arrayToCache)
		{
			int length = arrayToCache.Length;

			if (parallelArrayIndicies[length] < freeTableSize - 1)
			{
				nodeArrayStore[length * freeTableSize + parallelArrayIndicies[length]++ + 1] = arrayToCache;
			}
		}

		private INode[] TryToGetArrayFromStore(int length)
		{
			return parallelArrayIndicies[length] == emptyArrayStore ? new INode[length] : nodeArrayStore[length * freeTableSize + parallelArrayIndicies[length]--];
		}

		private void InsertIntoLinkerNode(LinkerNode linkerNode, KeyValueNode newKeyValuePair, int index, int addToBitmap)
		{
			int length = linkerNode.Nodes.Length;

			StoreArray(linkerNode.Nodes);

			INode[] newChildrenArray = TryToGetArrayFromStore(length + 1);

			Array.Copy(linkerNode.Nodes, 0, newChildrenArray, 0, index);
			Array.Copy(linkerNode.Nodes, index, newChildrenArray, index + 1, newChildrenArray.Length - index - 1);

			newChildrenArray[index] = newKeyValuePair;
			linkerNode.Bitmap |= (1 << addToBitmap);

			ItemCount++;

			linkerNode.Nodes = newChildrenArray;
		}

		private void ReplaceKeyWithLinkingNode(INode[] pNode, int pIndex, KeyValueNode oldKeyValuePair, KeyValueNode newKeyValuePair, int newKeyBitGroup, int newKeyBitIndex, int currentLevel)
		{
			int rootBitmap = root.Bitmap;
			int rootShiftAmount = (shiftAmount * (startingLevelCount + 1 - rootBitmap));
			int oldKeyBitGroup = this.hashEvaluate(oldKeyValuePair.Key) >> (rootShiftAmount + currentLevel * shiftAmount);
			int oldKeyBitIndex = oldKeyBitGroup & maskAmount;

			newKeyBitIndex = newKeyBitGroup & maskAmount;

			while (newKeyBitIndex == oldKeyBitIndex)
			{
				if (currentLevel < rootBitmap)
				{
					LinkerNode newChild = new LinkerNode(1, 1 << oldKeyBitIndex);

					pNode[pIndex] = newChild;

					pNode = newChild.Nodes;
					pIndex = 0;
				}
				else // Collision detected so insert at leaves bucket
				{
					if (comparer.Equals(newKeyValuePair.Key, oldKeyValuePair.Key))
					{
						throw new Exception("Key already exists.");
					}

					ItemCount++;

					const int completeBitmap = -1;

					LinkerNode newLeaf = new LinkerNode(2, completeBitmap);
					newLeaf.Nodes[0] = oldKeyValuePair;
					newLeaf.Nodes[1] = newKeyValuePair;

					pNode[pIndex] = newLeaf;

					return;
				}

				oldKeyBitGroup >>= shiftAmount;
				newKeyBitGroup >>= shiftAmount;

				oldKeyBitIndex = oldKeyBitGroup & maskAmount;
				newKeyBitIndex = newKeyBitGroup & maskAmount;

				currentLevel++;
			}

			// Promote Key to LinkingNode

			LinkerNode replacementNode = new LinkerNode(2, (1 << oldKeyBitIndex) | (1 << newKeyBitIndex));

			const int signShiftLocation = 31;

			int insertionLocation = ((oldKeyBitIndex - newKeyBitIndex) >> signShiftLocation) + 1;

			replacementNode.Nodes[insertionLocation] = oldKeyValuePair;
			replacementNode.Nodes[1 - insertionLocation] = newKeyValuePair;

			ItemCount++;

			pNode[pIndex] = replacementNode;

			// Can check here whether or not root has keys. If not, resize? Can keep a running counter here as well.

			if (ItemCount >= root.Nodes.Length * maxChildrenPerNode)
			{
				ResizeRoot();
			}
		}

		private void ResizeRoot()
		{
			LinkerNode newRoot = new LinkerNode(root.Nodes.Length * maxChildrenPerNode, root.Bitmap - 1);

			int rootShiftAmount = (shiftAmount * (startingLevelCount + 1 - root.Bitmap));

			for (int i = 0; i < root.Nodes.Length; i++)
			{
				LinkerNode currentRootChild = (LinkerNode)root.Nodes[i];

				int currElementIndex = 0;

				for (int j = 0; j < maxChildrenPerNode; j++)
				{
					if (((currentRootChild.Bitmap >> j) & 1) == 1)
					{
						INode currentNode = currentRootChild.Nodes[currElementIndex++];

						newRoot.Nodes[(j << rootShiftAmount) | i] = currentNode;
					}
				}
			}

			root = newRoot;
		}

		// Doesn't really matter how efficient this is because it's run so rarely.
		private void AppendLeavesBucket(LinkerNode leaves, KeyValueNode newKeyValuePair)
		{
			INode[] newArray = new INode[leaves.Nodes.Length + 1];

			Array.Copy(leaves.Nodes, newArray, leaves.Nodes.Length);

			newArray[leaves.Nodes.Length] = newKeyValuePair;

			leaves.Nodes = newArray;

			ItemCount++;
		}

		public bool Contains(TKey key)
		{
			return Get(key) != null;
		}

		public KeyValueNode Get(TKey key)
		{
			int rootShiftAmount = (shiftAmount * (startingLevelCount + 1 - root.Bitmap));
			int currBitGroup = this.hashEvaluate(key);
			int rootBitIndex = currBitGroup & (root.Nodes.Length - 1);

			LinkerNode linkerNode = root.Nodes[rootBitIndex] as LinkerNode;

			if (linkerNode != null)
			{
				currBitGroup >>= rootShiftAmount;

				for (int currentLevel = 1; currentLevel <= root.Bitmap; currBitGroup >>= shiftAmount, currentLevel++)
				{
					int indexGivenBitIndex = GetElementCountBeforeBitIndex(currBitGroup & maskAmount, linkerNode.Bitmap);

					KeyValueNode keyValueNode = linkerNode.Nodes[indexGivenBitIndex] as KeyValueNode;

					if (keyValueNode != null)
					{
						return comparer.Equals(keyValueNode.Key, key) ? keyValueNode : null;
					}
					else
					{
						linkerNode = (LinkerNode)linkerNode.Nodes[indexGivenBitIndex];
					}
				}

				foreach (KeyValueNode keyValueNode in linkerNode.Nodes) // Leaves
				{
					if (comparer.Equals(keyValueNode.Key, key))
					{
						return keyValueNode;
					}
				}

				return null;
			}
			else
			{
				return (KeyValueNode)root.Nodes[rootBitIndex];
			}
		}

		public IEnumerable<TKey> GetKeys()
		{
			return root.Nodes.SelectMany(GetKeysRecursive);
		}

		private static IEnumerable<TKey> GetKeysRecursive(INode node)
		{
			if (node == null) return new TKey[0];
			var keyValueNode = node as KeyValueNode;
			if (keyValueNode != null)
			{
				return new[] { keyValueNode.Key };
			}
			return ((LinkerNode)node).Nodes.Where(n => n != null).SelectMany(GetKeysRecursive);
		}

		public interface INode { }

		internal class LinkerNode : INode
		{
			public int Bitmap;
			public INode[] Nodes;

			public LinkerNode(int size, int bitmap) { Nodes = new INode[size]; Bitmap = bitmap; }
		}

		public class KeyValueNode : INode
		{
			public TKey Key;
			public TValue Value;

			public KeyValueNode(TKey key, TValue value) { Key = key; Value = value; }
		}

		private static int GetElementCountBeforeBitIndex(int bitIndex, int bitMap)
		{
			int bitsToCheck = bitMap << (maxChildrenPerNode - bitIndex - 1) << 1;

			// Start of Popcount operation
			bitsToCheck = bitsToCheck - ((bitsToCheck >> 1) & 0x55555555);
			bitsToCheck = (bitsToCheck & 0x33333333) + ((bitsToCheck >> 2) & 0x33333333);
			return (((bitsToCheck + (bitsToCheck >> 4)) & 0x0F0F0F0F) * 0x01010101) >> 24;
		}

		private static bool IsBitSet(int pos, int bitMap)
		{
			return (bitMap & (1 << pos)) != 0;
		}
	}
}
