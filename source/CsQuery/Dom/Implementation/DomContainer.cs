﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using CsQuery.ExtensionMethods;

namespace CsQuery.Implementation
{
    

    /// <summary>
    /// Base class for Dom object that contain other elements
    /// </summary>
    public abstract class DomContainer<T> : DomObject<T>, IDomContainer where T : IDomObject, IDomContainer, new()
    {
        public DomContainer()
        {
            
        }
        public DomContainer(IEnumerable<IDomObject> elements): base()
        {
            ChildNodesInternal.AddRange(elements);
        }


        /// <summary>
        /// Returns all children (including inner HTML as objects);
        /// </summary>
        public override INodeList ChildNodes
        {
            get
            {
                return ChildNodesInternal;
            }
        }

        /// <summary>
        /// The child nodes as a concete object.
        /// </summary>

        protected ChildNodeList ChildNodesInternal
        {
            get
            {
                if (_ChildNodes == null)
                {
                    _ChildNodes = new ChildNodeList(this);
                }
                return _ChildNodes;
            }
        }

        private ChildNodeList _ChildNodes;

        // Avoids creating children object when testing
        public override bool HasChildren
        {
            get
            {
                return ChildNodesInternal != null && ChildNodes.Count > 0;
            }
        }
        
        public override IDomObject FirstChild
        {
            get
            {
                if (HasChildren)
                {
                    return ChildNodes[0];
                }
                else
                {
                    return null;
                }
            }
        }
        public override IDomElement FirstElementChild
        {
            get
            {
                if (HasChildren)
                {
                    int index=0;
                    while (index < ChildNodes.Count && ChildNodes[index].NodeType != NodeType.ELEMENT_NODE)
                    {
                        index++;
                    }
                    if (index < ChildNodes.Count)
                    {
                        return (IDomElement)ChildNodes[index];
                    }
                }
                return null;
            }
        }
        public override IDomObject LastChild
        {
            get
            {
                return HasChildren ?
                    ChildNodes[ChildNodes.Count - 1] :
                    null;
            }
        }
        public override IDomElement LastElementChild
        {
            get
            {
                if (HasChildren)
                {
                    int index = ChildNodes.Count-1;
                    while (index >=0 && ChildNodes[index].NodeType != NodeType.ELEMENT_NODE)
                    {
                        index--;
                    }
                    if (index >=0)
                    {
                        return (IDomElement)ChildNodes[index];
                    }
                }
                return null;
            }
        }

        /// <summary>
        /// Appends a child.
        /// </summary>
        ///
        /// <param name="item">
        /// The item.
        /// </param>

        public override void AppendChild(IDomObject item)
        {
            ChildNodes.Add(item);
        }

        /// <summary>
        /// Appends a child without checking if it already exists.c
        /// </summary>
        ///
        /// <param name="item">
        /// The item.
        /// </param>

        internal override void AppendChildUnsafe(IDomObject item)
        {
            ChildNodesInternal.AddAlways(item);
        }
        public override void RemoveChild(IDomObject item)
        {
            ChildNodes.Remove(item);
        }
        public override void InsertBefore(IDomObject newNode, IDomObject referenceNode)
        {
            if (referenceNode.ParentNode != this)
            {
                throw new InvalidOperationException("The reference node is not a child of this node");
            }
            ChildNodes.Insert(referenceNode.Index, newNode);
        }
        public override void InsertAfter(IDomObject newNode, IDomObject referenceNode)
        {
            if (referenceNode.ParentNode != this)
            {
                throw new InvalidOperationException("The reference node is not a child of this node");
            }
            ChildNodes.Insert(referenceNode.Index + 1, newNode);
        }

        /// <summary>
        /// Get all child elements
        /// </summary>

        public override IEnumerable<IDomElement> ChildElements
        {
            get
            {
                if (HasChildren)
                {
                    foreach (IDomObject obj in ChildNodes)
                    {
                        var elm = obj as IDomElement;
                        if (elm != null)
                        {
                            yield return elm;
                        }
                    }
                }
                yield break;
            }
        }


        /// <summary>
        /// Renders the children.
        /// </summary>
        ///
        /// <param name="writer">
        /// The writer.
        /// </param>
        /// <param name="options">
        /// Options for controlling the operation.
        /// </param>

        //protected virtual void RenderChildren(TextWriter writer, DomRenderingOptions options)
        //{
        //    if (HasChildren)
        //    {
        //        foreach (IDomObject e in ChildNodes)
        //        {
        //            if (e.NodeType == NodeType.TEXT_NODE)
        //            {
        //                RenderChildTextNode(e, writer, options);
        //            }
        //            else
        //            {
        //                e.Render(writer, options);
        //            }
        //        }
        //    } 

        //}

        /// <summary>
        /// Renders the child text node. This can be overridden for non-default text handling by some
        /// element types (e.g. textarea)
        /// </summary>
        ///
        /// <param name="textNode">
        /// The text node.
        /// </param>
        /// <param name="sb">
        /// The stringbuilder.
        /// </param>

        //protected virtual void RenderChildTextNode(IDomObject textNode, TextWriter writer, DomRenderingOptions options)
        //{
        //    textNode.Render(writer, options);
        //}

        // Just didn't use the / and the +. A three character ID will permit over 250,000 possible children at each level
        // so that should be plenty
        

        public override int DescendantCount()
        {
            int count = 0;
            if (HasChildren)
            {
                foreach (IDomObject obj in ChildNodes)
                {
                    count += 1 + obj.DescendantCount();
                }
            }
            return count;
        }

       

        #region interface members
        IDomObject IDomObject.Clone()
        {
            return Clone();
        }

        IDomNode IDomNode.Clone()
        {
            return Clone();
        }

        object ICloneable.Clone()
        {
            return Clone();
        }
        #endregion
    }

}
