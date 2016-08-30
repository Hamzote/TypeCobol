﻿namespace TypeCobol.Compiler.Nodes {

	using System.Collections.Generic;
	using TypeCobol.Compiler.CodeElements;




public class DataDivision: Node, CodeElementHolder<DataDivisionHeader>, Parent<DataSection> {
	public DataDivision(DataDivisionHeader header): base(header) { }
	public override string ID { get { return "data-division"; } }
}

public abstract class DataSection: Node, CodeElementHolder<DataSectionHeader>, Child<DataDivision>, Parent<DataDefinition> {
	public DataSection(DataSectionHeader header): base(header) { }
	public virtual bool IsShared { get { return false; } }
}
public class FileSection: DataSection, CodeElementHolder<FileSectionHeader> {
	public FileSection(FileSectionHeader header): base(header) { }
	public override string ID { get { return "file"; } }
	public override bool IsShared { get { return true; } }
}
public class WorkingStorageSection: DataSection, CodeElementHolder<WorkingStorageSectionHeader> {
	public WorkingStorageSection(WorkingStorageSectionHeader header): base(header) { }
	public override string ID { get { return "working-storage"; } }
}
public class LocalStorageSection: DataSection, CodeElementHolder<LocalStorageSectionHeader> {
	public LocalStorageSection(LocalStorageSectionHeader header): base(header) { }
	public override string ID { get { return "local-storage"; } }
}
public class LinkageSection: DataSection, CodeElementHolder<LinkageSectionHeader> {
	public LinkageSection(LinkageSectionHeader header): base(header) { }
	public override string ID { get { return "linkage"; } }
	public override bool IsShared { get { return true; } }
}

public abstract class DataDefinition: Node, CodeElementHolder<DataDefinitionEntry>, Parent<DataDefinition>, Child<DataSection>, Named {
	public DataDefinition(DataDefinitionEntry entry): base(entry) { }
	public override string ID { get { return this.CodeElement().Name; } }
	public string Name { get { return this.CodeElement().Name; } }
}
public class DataDescription: DataDefinition, CodeElementHolder<DataDescriptionEntry>, Parent<DataDescription> {
	public DataDescription(DataDescriptionEntry entry): base(entry) { }
}
public class DataCondition: DataDefinition, CodeElementHolder<DataConditionEntry> {
	public DataCondition(DataConditionEntry entry): base(entry) { }
}
public class DataRedefines: DataDefinition, CodeElementHolder<DataRedefinesEntry> {
	public DataRedefines(DataRedefinesEntry entry): base(entry) { }
}
public class DataRenames: DataDefinition, CodeElementHolder<DataRenamesEntry> {
	public DataRenames(DataRenamesEntry entry): base(entry) { }
}
// [COBOL 2002]
public class TypeDefinition: DataDefinition, CodeElementHolder<TypeDefinitionEntry>, Parent<DataDescription> {
	public TypeDefinition(TypeDefinitionEntry entry): base(entry) { }
	public bool IsStrong { get; internal set; }
}
// [/COBOL 2002]



} // end of namespace TypeCobol.Compiler.Nodes