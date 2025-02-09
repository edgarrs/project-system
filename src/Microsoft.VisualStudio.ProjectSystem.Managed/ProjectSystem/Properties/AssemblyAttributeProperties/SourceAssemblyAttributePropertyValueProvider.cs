﻿// Copyright (c) Microsoft.  All Rights Reserved.  Licensed under the Apache License, Version 2.0.  See License.txt in the project root for license information.

using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using System.Threading.Tasks;

using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Editing;

#nullable disable

namespace Microsoft.VisualStudio.ProjectSystem.Properties
{
    /// <summary>
    /// This class represents properties corresponding to assembly attributes that are stored in the source code of the project.
    /// </summary>
    internal class SourceAssemblyAttributePropertyValueProvider
    {
        private readonly string _assemblyAttributeFullName;
        private readonly Func<ProjectId> _getActiveProjectId;
        private readonly Workspace _workspace;
        private readonly IProjectThreadingService _threadingService;

        public SourceAssemblyAttributePropertyValueProvider(
            string assemblyAttributeFullName,
            Func<ProjectId> getActiveProjectId,
            Workspace workspace,
            IProjectThreadingService threadingService)
        {
            _assemblyAttributeFullName = assemblyAttributeFullName;
            _getActiveProjectId = getActiveProjectId;
            _workspace = workspace;
            _threadingService = threadingService;
        }

        private Project GetActiveProject()
        {
            ProjectId activeProjectId = _getActiveProjectId();
            if (activeProjectId == null)
            {
                return null;
            }

            return _workspace.CurrentSolution.Projects.SingleOrDefault((p, id) => p.Id == id, activeProjectId);
        }

        /// <summary>
        /// Gets the value of the property from the source assembly attribute.
        /// </summary>
        public async Task<string> GetPropertyValueAsync()
        {
            Project project = GetActiveProject();
            if (project == null)
            {
                return null;
            }

            AttributeData attribute = await GetAttributeAsync(_assemblyAttributeFullName, project);

            return attribute?.ConstructorArguments.FirstOrDefault().Value?.ToString();
        }

        /// <summary>
        /// Sets the value of the property in the source assembly attribute.
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public async Task SetPropertyValueAsync(string value)
        {
            Project project = GetActiveProject();
            if (project == null)
            {
                return;
            }

            AttributeData attribute = await GetAttributeAsync(_assemblyAttributeFullName, project);
            if (attribute == null)
            {
                return;
            }

            SyntaxNode attributeNode = await attribute.ApplicationSyntaxReference.GetSyntaxAsync();
            var syntaxGenerator = SyntaxGenerator.GetGenerator(project);
            IReadOnlyList<SyntaxNode> arguments = syntaxGenerator.GetAttributeArguments(attributeNode);

            // The attributes of interest to us have one argument. If there are more then we have broken code - don't change that.
            if (arguments.Count == 1)
            {
                SyntaxNode argumentNode = arguments[0];
                SyntaxNode newNode;
                if (attribute.AttributeConstructor.Parameters.FirstOrDefault()?.Type.SpecialType == SpecialType.System_Boolean)
                {
                    newNode = syntaxGenerator.AttributeArgument(bool.Parse(value) ? syntaxGenerator.TrueLiteralExpression() : syntaxGenerator.FalseLiteralExpression());
                }
                else
                {
                    newNode = syntaxGenerator.AttributeArgument(syntaxGenerator.LiteralExpression(value));
                }

                newNode = newNode.WithTriviaFrom(argumentNode);
                DocumentEditor editor = await DocumentEditor.CreateAsync(project.GetDocument(attributeNode.SyntaxTree));
                editor.ReplaceNode(argumentNode, newNode);

                // Apply changes needs to happen on the UI Thread.
                await _threadingService.SwitchToUIThread();
                _workspace.TryApplyChanges(editor.GetChangedDocument().Project.Solution);
            }
        }

        /// <summary>
        /// Get the attribute corresponding to the given property from the given project.
        /// </summary>
        private static async Task<AttributeData> GetAttributeAsync(string assemblyAttributeFullName, Project project)
        {
            Compilation compilation = await project.GetCompilationAsync();
            ImmutableArray<AttributeData> assemblyAttributes = compilation.Assembly.GetAttributes();

            INamedTypeSymbol attributeTypeSymbol = compilation.GetTypeByMetadataName(assemblyAttributeFullName);
            if (attributeTypeSymbol == null)
            {
                return null;
            }

            return assemblyAttributes.FirstOrDefault((data, symbol) => data.AttributeClass.Equals(symbol), attributeTypeSymbol);
        }
    }
}
