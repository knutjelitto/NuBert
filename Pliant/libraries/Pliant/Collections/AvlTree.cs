﻿using System;
using System.Collections.Generic;

namespace Pliant.Collections
{
    public class AvlTree<T>
        where T : IComparable<T>
    {
        public IEnumerator<T> GetEnumerator()
        {
            return GetEnumerator(this._root);
        }

        public void Insert(T key)
        {
            this._root = Insert(this._root, key);
        }

        private static int GetBalanceFactor(AvlNode node)
        {
            if (node == null)
            {
                return 0;
            }

            return Height(node.Left) - Height(node.Right);
        }

        private static int Height(AvlNode node)
        {
            if (node == null)
            {
                return 0;
            }

            return node.Height;
        }

        private static AvlNode RotateLeft(AvlNode x)
        {
            var y = x.Right;
            var temp = y.Left;

            // rotation
            y.Left = x;
            x.Right = temp;

            // update height
            x.Height = Math.Max(Height(x.Left), Height(x.Right)) + 1;
            y.Height = Math.Max(Height(y.Left), Height(y.Right)) + 1;

            // new root
            return y;
        }

        private static AvlNode RotateRight(AvlNode y)
        {
            var x = y.Left;
            var temp = x.Right;

            // rotation
            x.Right = y;
            y.Left = temp;

            // update height
            y.Height = Math.Max(Height(y.Left), Height(y.Right)) + 1;
            x.Height = Math.Max(Height(x.Left), Height(x.Right)) + 1;

            // new root
            return x;
        }

        private IEnumerator<T> GetEnumerator(AvlNode node)
        {
            if (node == null)
            {
                yield break;
            }

            var leftTree = GetEnumerator(node.Left);
            while (leftTree.MoveNext())
            {
                yield return leftTree.Current;
            }

            yield return node.Key;

            var rightTree = GetEnumerator(node.Right);
            while (rightTree.MoveNext())
            {
                yield return rightTree.Current;
            }
        }

        private AvlNode Insert(AvlNode node, T key)
        {
            var isLeaf = node == null;
            if (isLeaf)
            {
                return new AvlNode(key);
            }

            // do a binary search tree insertion recursively
            if (key.CompareTo(node.Key) < 0)
            {
                node.Left = Insert(node.Left, key);
            }
            else
            {
                node.Right = Insert(node.Right, key);
            }

            // update height
            node.Height = Math.Max(
                              Height(node.Left),
                              Height(node.Right)) + 1;

            // at each level of recursion, check if the tree is unbalanced
            var balanceFactor = GetBalanceFactor(node);

            if (-1 <= balanceFactor && balanceFactor <= 1)
            {
                return node;
            }

            // Left Left
            if (balanceFactor > 1 && key.CompareTo(node.Left.Key) < 0)
            {
                return RotateRight(node);
            }

            // Right Right
            if (balanceFactor < -1 && key.CompareTo(node.Right.Key) > 0)
            {
                return RotateLeft(node);
            }

            // Left Right
            if (balanceFactor > 1 && key.CompareTo(node.Left.Key) > 0)
            {
                node.Left = RotateLeft(node.Left);
                return RotateRight(node);
            }

            // Right Left
            if (balanceFactor < -1 && key.CompareTo(node.Right.Key) < 0)
            {
                node.Right = RotateRight(node.Right);
                return RotateLeft(node);
            }

            return node;
        }

        private AvlNode _root;

        #region  not sortable (modify ReSharper template to catch these cases)

        private class AvlNode
        {
            public AvlNode(T key)
            {
                Key = key;
                Height = 1;
            }

            public int Height { get; set; }
            public T Key { get; }

            public AvlNode Left { get; set; }

            public AvlNode Right { get; set; }
        }

        #endregion
    }
}