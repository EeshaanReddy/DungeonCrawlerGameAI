using System;
using System.Collections.Generic;

namespace Misc
{
    public class TreeNode<T> where T : IComparable<T>
    {
        public TreeNode<T> left = null;

        public TreeNode<T> right = null;

        public List<T> values = new List<T>();

        public bool isRed = true;

        public TreeNode<T> parent;

        public TreeNode (T _value)
        {
            values.Add(_value);
        }

        public void remove()
        {
            this.parent.removeChild(this);
        }

        public void changeChild(TreeNode<T> child)
        {
            if (values[0].CompareTo(child.values[0]) < 0)
            {
                right = child;
            }
            else
            {
                left = child;
            }
        }

        private void removeChild(TreeNode<T> child)
        {
            if (this.left.Equals(child))
            {
                this.left = null;
            }
            else
            {
                this.right = null;
            }
        }
    }
}