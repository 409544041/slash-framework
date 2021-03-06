﻿// --------------------------------------------------------------------------------------------------------------------
// <copyright file="BoolComponent.cs" company="Slash Games">
//   Copyright (c) Slash Games. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Slash.Tools.BlueprintEditor.SampleProject.EntityComponents
{
    using Slash.Collections.AttributeTables;
    using Slash.ECS.Components;
    using Slash.ECS.Inspector.Attributes;

    [InspectorComponent(Description = "A test component with a boolean attribute.")]
    public class BoolComponent : IEntityComponent
    {
        #region Constants

        /// <summary>
        ///   Attribute: Test attribute.
        /// </summary>
        public const string AttributeValue = "BoolComponent.Value";

        /// <summary>
        ///   Attribute default: Test attribute.
        /// </summary>
        public const bool DefaultValue = false;

        #endregion

        #region Fields

        private bool value = DefaultValue;

        #endregion

        #region Public Properties

        /// <summary>
        ///   Test attribute.
        /// </summary>
        [InspectorBool(AttributeValue, Description = "Test bool attribute", Default = DefaultValue)]
        public bool Value
        {
            get
            {
                return this.value;
            }
            set
            {
                this.value = value;
            }
        }

        #endregion

        #region Public Methods and Operators

        /// <summary>
        ///   Initializes this component with the data stored in the specified
        ///   attribute table.
        /// </summary>
        /// <param name="attributeTable">Component data.</param>
        public void InitComponent(IAttributeTable attributeTable)
        {
            attributeTable.TryGetBool(AttributeValue, out this.value);
        }

        #endregion
    }
}