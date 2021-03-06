﻿// --------------------------------------------------------------------------------------------------------------------
// <copyright file="InspectorConditionalPropertyAttribute.cs" company="Slash Games">
//   Copyright (c) Slash Games. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Slash.ECS.Inspector.Attributes
{
    using System;

    /// <summary>
    ///   Property inspector should only be shown if condition is met.
    /// </summary>
    [AttributeUsage(AttributeTargets.Property)]
    public class InspectorConditionalPropertyAttribute : Attribute
    {
        #region Constructors and Destructors

        /// <summary>
        ///   Property inspector should only be shown if condition is met.
        /// </summary>
        /// <param name="conditionName">Attribute table key of the condition in the entity configuration.</param>
        /// <param name="requiredConditionValue">
        ///   Attribute table value of the condition in the entity configuration for the inspector to be shown
        /// </param>
        public InspectorConditionalPropertyAttribute(object conditionName, object requiredConditionValue)
        {
            this.ConditionKey = conditionName;
            this.RequiredConditionValue = requiredConditionValue;
        }

        /// <summary>
        ///   Default constructor.
        /// </summary>
        public InspectorConditionalPropertyAttribute()
        {
        }

        #endregion

        #region Public Properties

        /// <summary>
        ///   Attribute table key of the condition in the entity configuration.
        /// </summary>
        public object ConditionKey { get; set; }

        /// <summary>
        ///   Attribute table value of the condition in the entity configuration for the inspector to be shown.
        /// </summary>
        public object RequiredConditionValue { get; set; }

        #endregion

        #region Public Methods and Operators

        /// <summary>
        ///   Checks if the specified value fulfills the condition.
        /// </summary>
        /// <param name="value">Value to check.</param>
        /// <returns>True if the specified value fulfills the condition; otherwise, false.</returns>
        public virtual bool IsFulfilled(object value)
        {
            return Equals(value, this.RequiredConditionValue);
        }

        #endregion
    }
}