using System;
using System.IO.Enumeration;
using UnityEditor;
using UnityEngine;
using UnityEngine.Assertions;

namespace Misc
{
    public class RBT<T> where T : IComparable<T>
    {
        public TreeNode<T> root = null;

        public int Count;

        //deletes and returns min value in the tree. takes the one added first in cases of ties, returns default value of T
        //for an empty tree
        public T deleteMin()
        {
            T retval = default(T);
            TreeNode<T> curNode = root;
            while (curNode.left != null)
            {
                curNode = curNode.left;
            }
            if (curNode == null) return retval;
            retval = curNode.values[0];
            delete(curNode.values[0]);
            return retval;
        }

        //insert the passed element into the tree, maintaining RBT properties
        public void insert(T element)
        {
            root = insert(root, element);
            if (root.isRed)
            {
                root.isRed = false;
            }
            Count++;
        }

        //delete the passed element from the tree, maintaining RBT properties
        public void delete(T element)
        {
            TreeNode<T> cur = get(element);
            if (cur == null)
            {
                return;
            }
            if (cur.values.Count > 1)
            {
                cur.values.Remove(element);
            }
            else if (cur.isRed && cur.left == null && cur.right == null)
            {
                cur.remove();
            }
            else if (root.Equals(cur) && cur.left == null && cur.right == null)
            {
                root = null;
            }
            else
            {
                getSuccessorAndDelete(cur);
                if (root.isRed)
                {
                    root.isRed = false;
                }
            }
            Count--;
        }

        //returns true if the tree contains the element, false otherwise
        public bool Contains(T element)
        {
            TreeNode<T> matchingNode = get(element);
            if (matchingNode != null && matchingNode.values.Contains(element))
                return true;
            return false;
        }
        
        //finds and returns the tree node that contains the passed value
        //returns null if such a node does not exist
        public TreeNode<T> get(T element)
        {
            if (element == null)
            {
                return null;
            }
            TreeNode<T> cur = root;
            while (true)
            {
                if (cur == null)
                {
                    break;
                }
                if (cur.values[0].CompareTo(element) < 0)
                {
                    cur = cur.right;
                }
                else if (cur.values[0].CompareTo(element) > 0)
                {
                    cur = cur.left;
                }
                else
                {
                    break;
                }
            }

            if (cur != null)
            {
                foreach (T val in cur.values)
                {
                    if (val.Equals(element))
                    {
                        return cur;
                    }
                }
            }

            return null;
        }

        /*finds the right successor to the passed node, or the left successor if there is no right successor, while
        tracking the nearest red node to that successor. Once the successor is found, passes the red link down
        for deletion, swaps the successor up to the start node, then removes the start node and fixes the tree
        on the way back up */
        private void getSuccessorAndDelete(TreeNode<T> start)
        {
            TreeNode<T> nearestRed = findRedNearestStart(start);
            TreeNode<T> cur = start;
            if (start.right != null)
            {
                cur = start.right;
                //find successor node to start - leftmost node in right subtree, continue to track nearest red node
                while (cur.left != null)
                {
                    if (cur.isRed)
                    {
                        nearestRed = cur;
                    }
                    cur = cur.left;
                }
            }
            else if (start.left != null)
            {
                cur = start.left;
                //find successor node to start - rightmost node in left subtree, continue to track nearest red node
                while (cur.right != null)
                {
                    if (cur.isRed)
                    {
                        nearestRed = cur;
                    }
                    cur = cur.right;
                }
            }
            if (cur.isRed)
            {
                start.values = cur.values;
                cur.remove();
            }
            else
            {
                while (nearestRed != cur)
                {
                    colorFlip(nearestRed);
                    if (nearestRed.values[0].CompareTo(cur.values[0]) < 0)
                    {
                        nearestRed = nearestRed.right;
                    }
                    else
                    {
                        nearestRed = nearestRed.left;
                    }
                }
                if (root.isRed)
                {
                    root.isRed = false;
                }
                start.values = cur.values;
                TreeNode<T> temp = cur.parent;
                cur.remove();
                fixUp(temp);
            }
        }

        //moves up the tree from the passed node, fixing any RBT rule violations detected.
        private void fixUp(TreeNode<T> cur)
        {
            bool neededFix;
            TreeNode<T> last = cur;
            while (cur != null)
            {
                cur = fixTree(in cur, out neededFix);
                if (neededFix && last != null)
                {
                    cur = last;
                }
                else
                {
                    last = cur;
                    cur = cur.parent;
                }
            }
        }

        //finds and returns the red node closest to the passed node, or the root if there is none
        private TreeNode<T> findRedNearestStart(TreeNode<T> start)
        {
            TreeNode<T> cur = root;
            TreeNode<T> nearestRed = root;
            while (!cur.Equals(start))
            {
                if (start.values[0].CompareTo(cur.values[0]) < 0)
                {
                    cur = cur.left;
                }
                else
                {
                    cur = cur.right;
                }
                if (cur.isRed)
                {
                    nearestRed = cur;
                }
            }
            return nearestRed;
        }

        //recursive helper function for insert, adding the passed element to the subtree whose root
        //is curNode
        private TreeNode<T> insert(TreeNode<T> curNode, T element)
        {
            if (curNode == null)
            {
                return new TreeNode<T>(element);
            }

            if (element.CompareTo(curNode.values[0]) < 0)
            {
                curNode.left = insert(curNode.left, element);
                curNode.left.parent = curNode;
            }
            else if (element.CompareTo(curNode.values[0]) > 0)
            {
                curNode.right = insert(curNode.right, element);
                curNode.right.parent = curNode;
            }
            else
            {
                curNode.values.Add(element);
                return curNode;
            }

            return fixTree(curNode);
        }

        //detects and fixes any problems stemming directly from the passed node 
        //does not detect or fix issues further up or down the tree
        private TreeNode<T> fixTree(TreeNode<T> curNode)
        {
            if (isRed(curNode.right) && !isRed(curNode.left))
            {
                curNode = leftRotate(curNode);

            }

            if (isRed(curNode.left.left) && isRed(curNode.left))
            {
                curNode = rightRotate(curNode);
            }
            
            if (isRed(curNode.left) && isRed(curNode.right))
            {
                colorFlip(curNode);
            }

            return curNode;
        }
        
        //overload for fix tree that returns whether a fix was required via the output parameter
        private TreeNode<T> fixTree(in TreeNode<T> cur, out bool neededFix)
        {
            TreeNode<T> curNode = cur;
            neededFix = false;
            if (isRed(curNode.right) && !isRed(curNode.left))
            {
                curNode = leftRotate(curNode);
                neededFix = true;
            }

            if (curNode.left != null)
            {
                if (isRed(curNode.left.left) && isRed(curNode.left))
                {
                    curNode = rightRotate(curNode);
                    neededFix = true;
                }
            }

            if (isRed(curNode.left) && isRed(curNode.right))
            {
                colorFlip(curNode);
                neededFix = true;
            }

            return curNode;
        }

        //determines whether the passed node is red. null links are black.
        private bool isRed(TreeNode<T> node)
        {
            if (node == null)
            {
                return false;
            }

            return node.isRed;
        }

        //swaps the colors of the passed nodes
        private void swapColors(TreeNode<T> node1, TreeNode<T> node2)
        {
            (node1.isRed, node2.isRed) = (node2.isRed, node1.isRed);
        }

        //inverts the colors of curNode and its children. Only perform this operation when
        //curNode and its children have opposite colors and only when curNode has two children.
        private void colorFlip(TreeNode<T> curNode)
        {
            curNode.left.isRed = !curNode.left.isRed;
            curNode.right.isRed = !curNode.right.isRed;
            curNode.isRed = !curNode.isRed;
        }

        //performs a left rotation on the passed node, moving it down to the left and its right child up
        private TreeNode<T> leftRotate(TreeNode<T> curNode)
        {
            TreeNode<T> lchild = curNode.right.left;
            TreeNode<T> rchild = curNode.right;
            rchild.left = curNode;
            rchild.parent = curNode.parent;
            curNode.parent = rchild;
            curNode.right = lchild;
            if (lchild != null)
            {
                lchild.parent = curNode;
            }
            if (curNode.Equals(root))
            {
                root = rchild;
            }
            else
            {
                rchild.parent.changeChild(rchild);
            }
            swapColors(rchild, rchild.left);
            return rchild;
        }
        
        //performs a right rotation on the passed node, moving it down to the right and its left child up
        private TreeNode<T> rightRotate(TreeNode<T> curNode)
        {
            TreeNode<T> lchild = curNode.left;
            TreeNode<T> rchild = curNode.left.right;
            lchild.right = curNode;
            lchild.parent = curNode.parent;
            curNode.parent = lchild;
            curNode.left = rchild;
            if (rchild != null)
            {
                rchild.parent = curNode;
            }
            if (curNode.Equals(root))
            {
                root = lchild;
            }
            else
            {
                lchild.parent.changeChild(lchild);
            }
            swapColors(lchild, lchild.right);
            return lchild;
        }
    }
}