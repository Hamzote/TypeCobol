﻿using System;
using System.Collections;
using System.Collections.Generic;
using Antlr4.Runtime;
using TypeCobol.Compiler.AntlrUtils;
using TypeCobol.Compiler.CodeElements;
using TypeCobol.Compiler.CodeElements.Expressions;
using TypeCobol.Compiler.CodeModel;
using TypeCobol.Compiler.Parser;
using TypeCobol.Compiler.Parser.Generated;
using TypeCobol.Compiler.Nodes;

namespace TypeCobol.Compiler.Diagnostics
{

	class Cobol2002TypeDefChecker: CodeElementListener {
		public IList<Type> GetCodeElements() {
			return new List<Type> {typeof(TypeDefinitionEntry)};
		}

		public void OnCodeElement(CodeElement e, ParserRuleContext c) {
			var typedef = e as TypeDefinitionEntry;
			var context = c as CodeElementsParser.DataDescriptionEntryContext;

			if (typedef.LevelNumber.Value != 1) {
				string message = "TYPEDEF clause can only be specified for level 01 entries";
				DiagnosticUtils.AddError(typedef, message, context.cobol2002TypedefClause());
			}
			if (typedef.Picture != null && typedef.DataType.IsStrong) {
				string message = "Elementary TYPEDEF cannot be STRONG";
				string rulestack = new RuleStackBuilder().GetRuleStack(context.cobol2002TypedefClause());
				DiagnosticUtils.AddError(typedef, message, ParseTreeUtils.GetFirstToken(context.cobol2002TypedefClause().STRONG()), rulestack);
			}
			if (context.redefinesClause() != null) {
				string message = "REDEFINES clause cannot be specified with TYPEDEF clause";
				DiagnosticUtils.AddError(typedef, message, context.redefinesClause());
			}
			if (typedef.IsExternal) {
				string message = "EXTERNAL clause cannot be specified with TYPEDEF clause";
				foreach (var external in context.externalClause())
					DiagnosticUtils.AddError(typedef, message, external);
			}
			if (typedef.DataType.IsStrong && typedef.InitialValue != null) {
				string message = "Strong Typedef can't contains value clause:";
				foreach (var valeuClause in context.valueClause())
					DiagnosticUtils.AddError(typedef, message, valeuClause);
			}
		}
	}

    class Cobol2002TypeDefChecker2 : NodeListener
    {
        public IList<Type> GetNodes()
        {
            return new List<Type> { typeof(VariableWriter) };
        }

		public void OnNode(Node node, ParserRuleContext context, CodeModel.Program program) {
            var ce = node.CodeElement;
            if (ce is VariableWriter == false)
                return;
            if (ce is InitializeStatement || ce is MoveStatement || ce is ReadStatement || ce is ReleaseStatement
                                                                                            //SetStatement is not specified in our specs, but as
                                                                                            // a level 88 variable can't be strongly typed
                                                                                            //we don't need to check this case
                                                                                            //   +
                                                                                            // SET myBool TO TRUE where myBool is of type BOOL need to works
                || ce is ReturnStatement || ce is RewriteStatement || ce is WriteStatement || ce is SetStatement)
                return;

            
            var variablesWritten = ((VariableWriter)ce).VariablesWritten;
            SymbolTable symbolTable = program.SymbolTable;
		    foreach (var varWritten in variablesWritten)
		    {
                IList<Named> variablesNameds = symbolTable.GetVariable(varWritten.Key);
		        foreach (var variableNamed in variablesNameds)
		        {
		            if (variableNamed is DataDefinition)
		            {
                        DataType dataType = GetFirstStrongDataType(symbolTable, (DataDefinition) variableNamed);
		                if (dataType != null)
		                {
                            DiagnosticUtils.AddError(ce, "Strongly typed variable can't be used as a receiving operand of " + ce.Type);
                        }
                    }
		        }

		    }
        }

        //TODO need to move this method so we can reuse it
        private DataType GetFirstStrongDataType(SymbolTable table, DataDefinition symbol)
        {
            var parent = symbol;
            while (parent != null)
            {
                DataType dataType = GetTypeDefinition(table, parent);
                if (dataType.IsStrong) return dataType;
                parent = parent.Parent as DataDefinition;
            }
            return null;
        }

        //TODO need to move this method so we can reuse it
        private DataType GetTypeDefinition(SymbolTable table, Named symbol)
        {
            var data = symbol as DataDefinition;
            if (data != null)
            {
                var entry = (DataDescriptionEntry)data.CodeElement;
                if (entry.CustomType == null) return entry.DataType;//not a custom type
            }
            Typed typed = symbol as Typed;
            if (typed == null) return null;// symbol untyped
            var types = table.GetTypes(typed);
            if (types.Count != 1) return null;// symbol type not found or ambiguous
            return types[0].DataType;
        }
    }
}