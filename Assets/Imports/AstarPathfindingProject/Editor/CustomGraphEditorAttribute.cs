using System;

namespace Pathfinding
{
    /// <summary>Added to editors of custom graph types</summary>
    [AttributeUsage(AttributeTargets.All, Inherited = false, AllowMultiple = true)]
    public class CustomGraphEditorAttribute : Attribute
    {
        /// <summary>Name displayed in the inpector</summary>
        public string displayName;

        /// <summary>Type of the editor for the graph</summary>
        public Type editorType;

        /// <summary>Graph type which this is an editor for</summary>
        public Type graphType;

        public CustomGraphEditorAttribute(Type t, string displayName)
        {
            graphType = t;
            this.displayName = displayName;
        }
    }
}