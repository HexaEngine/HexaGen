// Copyright (c) Alexandre Mutel. All rights reserved.
// Licensed under the BSD-Clause 2 license.
// See license.txt file in the project root for full license information.
namespace HexaGen.CppAst.Model.Interfaces
{
    using HexaGen.CppAst.Model.Metadata;

    /// <summary>
    /// Base interface for all Cpp declaration.
    /// </summary>
    public interface ICppDeclaration : ICppElement
    {
        /// <summary>
        /// Gets or sets the comment attached to this element. Might be null.
        /// </summary>
        CppComment Comment { get; set; }
    }
}