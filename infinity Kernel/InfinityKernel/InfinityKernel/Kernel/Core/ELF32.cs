
/*
 * Copyright (c) 2013 GruntXProductions Grunty OS Project
 * 
 * This program is free software; you can redistribute it and/or
 * modify it under the terms of the GNU General Public License
 * as published by the Free Software Foundation; either version 2
 * of the License, or (at your option) any later version.
 * 
 * This program is distributed in the hope that it will be useful,
 * but WITHOUT ANY WARRANTY; without even the implied warranty of
 * MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
 * GNU General Public License for more details.
 * 
 * You should have received a copy of the GNU General Public License
 * along with this program; if not, write to the Free Software
 * Foundation, Inc., 51 Franklin Street, Fifth Floor, Boston, MA  02110-1301, USA.
 */

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.InteropServices;
using GruntyOS.IO;

namespace GruntyOS.Core
{

    [StructLayout(LayoutKind.Explicit, Pack = 1)]
    struct Elf32_sym
    {
        [FieldOffset(0)]
        public uint st_name;
        [FieldOffset(4)]
        public uint st_value;
        [FieldOffset(8)]
        public uint st_size;
        [FieldOffset(12)]
        public byte st_info;
        [FieldOffset(13)]
        public byte st_other;
        [FieldOffset(14)]
        public ushort st_shndx;
    }

    [StructLayout(LayoutKind.Explicit, Pack = 1)]
    struct Elf32_Rel
    {
        [FieldOffset(0)]
        public uint r_offset;
        [FieldOffset(4)]
        public uint r_info;
    }

    [StructLayout(LayoutKind.Explicit, Pack = 1)]
    struct Elf32_Rela
    {
        [FieldOffset(0)]
        public uint r_offset;
        [FieldOffset(4)]
        public uint r_info;
        [FieldOffset(8)]
        public int r_addend;
    }
    [StructLayout(LayoutKind.Explicit, Pack = 1)]
    public unsafe struct Elf32_Ehdr
    {
        [FieldOffset(0)]
        public fixed byte e_ident[16];
        [FieldOffset(16)]
        public ushort e_type;
        [FieldOffset(18)]
        public ushort e_machine;
        [FieldOffset(20)]
        public uint e_version;
        [FieldOffset(24)]
        public uint e_entry;
        [FieldOffset(28)]
        public uint e_phoff;
        [FieldOffset(32)]
        public uint e_shoff;
        [FieldOffset(36)]
        public uint e_flags;
        [FieldOffset(40)]
        public ushort e_ehsize;
        [FieldOffset(42)]
        public ushort e_phentsize;
        [FieldOffset(44)]
        public ushort e_phnum;
        [FieldOffset(46)]
        public ushort e_shentsize;
        [FieldOffset(48)]
        public ushort e_shnum;
        [FieldOffset(50)]
        public ushort e_shstrndx;
    }

    [StructLayout(LayoutKind.Explicit, Pack = 1)]
    public unsafe struct Elf32_Shdr
    {
        [FieldOffset(0)]
        public uint sh_name;
        [FieldOffset(4)]
        public uint sh_type;
        [FieldOffset(8)]
        public uint sh_flags;
        [FieldOffset(12)]
        public uint sh_addr;
        [FieldOffset(16)]
        public uint sh_offset;
        [FieldOffset(20)]
        public uint sh_size;
        [FieldOffset(24)]
        public uint sh_link;
        [FieldOffset(28)]
        public uint sh_info;
        [FieldOffset(32)]
        public uint sh_addralign;
        [FieldOffset(36)]
        public uint sh_entsize;
    }
    public class SectionInfo
    {
        public string Name;
        public uint Type;
        public uint Offset;
        public uint Size;
        public uint NameIndex;
        public uint Align;
        public uint Address;
    }
    public class SymbolInfo
    {
        public string Name;
        public uint Value;
        public uint Size;
        public uint NameIndx;
        public uint Type;
        public byte Other;
    }
    public class RelocInfo
    {
        public uint Symbol;
        public uint Address;
    }
    public class SectionData
    {
        public string Name;
        public uint LoadAddress;
        public byte[] data;
    }

    public unsafe class ELF32
    {

        public byte[] text;
        public byte[] data;
        public byte[] rodata;
        public uint EntryPoint;
        private static byte B;
        public bool dataFirst = false, codeFirst = false, dataLoaded = false;
        public List<SectionInfo> Info = new List<SectionInfo>();
        public List<SymbolInfo> Symbols = new List<SymbolInfo>();
        public List<SectionData> sectionData = new List<SectionData>();
        public List<RelocInfo> relocInfo = new List<RelocInfo>();
        public void Relocate(uint align)
        {
           

        }
           
        private BinaryReader br;
        public string getSymbolName(uint strindx)
        {
            uint oldp = br.BaseStream.Pointer;
            string name = "";
            for (int i = 0; i < Info.Count; i++)
            {
                if (Info[i].Name == ".strtab")
                {
                    br.BaseStream.Pointer= (Info[i].Offset + strindx);
                    while (true)
                    {
                        byte b = br.ReadByte();
                        if (b != 0)
                            name += ((char)b).ToString();
                        else
                            break;
                    }
                }
            }
            br.BaseStream.Pointer= oldp;
            return name;
        }
        public void Invoke(string function)
        {
            for (int i = 0; i < Symbols.Count; i++)
            {
               // if (function == Symbols[i].Name)
                //    Executable.LoadELF(this, Symbols[i].Value);
            }
        }
        public uint GetAddress(string sec)
        {
            for (int i = 0; i < this.Info.Count; i++)
            {
                if (Info[i].Name == sec)
                    return Info[i].Address;
            }
            return 0;
        }
        public void doLoad(Stream stm)
        {
            Elf32_Ehdr* header = (Elf32_Ehdr*)stdio.malloc((uint)sizeof(Elf32_Ehdr));
            Elf32_Shdr* sectionHeader = (Elf32_Shdr*)stdio.malloc((uint)sizeof(Elf32_Shdr));
            stdio.memcopy(stm, header,(uint)sizeof(Elf32_Ehdr));
            stm.Pointer = header->e_shoff;
            stdio.memcopy(stm, sectionHeader, (uint)sizeof(Elf32_Shdr));
        }
        public void Load(Stream stm)
        {
            stdio.printf("Loading ELF\n");
            List<byte> rd = new List<byte>();
            br = new BinaryReader(stm);
            rodata = new byte[10];
            data = new byte[10];
            byte* header = (byte*)Cosmos.Core.Heap.MemAlloc((uint)sizeof(Elf32_Ehdr));
            byte* section_header = (byte*)Cosmos.Core.Heap.MemAlloc((uint)sizeof(Elf32_Shdr));
            byte* symbol_buff = (byte*)Cosmos.Core.Heap.MemAlloc((uint)sizeof(Elf32_sym));
            byte* rel_buff = (byte*)Cosmos.Core.Heap.MemAlloc((uint)sizeof(Elf32_Rel));
            data = new byte[] { 0 };
            int t_size = sizeof(Elf32_Ehdr);
            for (int i = 0; i < t_size; i++)
            {
                byte b = (byte)br.ReadByte();
                B = (byte)b;
                ((byte*)header)[i] = (byte)B;
            }
            Elf32_Ehdr* hdr = (Elf32_Ehdr*)header;
            List<uint> StringTables = new List<uint>();
            List<uint> names = new List<uint>();
            br.BaseStream.Pointer = hdr->e_shoff;
            EntryPoint = hdr->e_entry;
            stdio.printf("Parsing...");
            for (int i = 0; i < hdr->e_shnum; i++)
            {
                Elf32_Shdr* sechdr = (Elf32_Shdr*)section_header;

                for (int p = 0; p < sizeof(Elf32_Shdr); p++)
                {
                    section_header[p] = br.ReadByte();
                }
                if (sechdr->sh_type == (uint)3)
                {
                    StringTables.Add(sechdr->sh_offset);
                }
                names.Add(sechdr->sh_name);
                SectionInfo sinfo = new SectionInfo();
                sinfo.Type = sechdr->sh_type;
                sinfo.Offset = sechdr->sh_offset;
                sinfo.NameIndex = sechdr->sh_name;
                sinfo.Size = sechdr->sh_size;
                sinfo.Align = sechdr->sh_addralign;
                sinfo.Address = sechdr->sh_addr;
                Info.Add(sinfo);
            }


            for (int cu = 0; cu < Info.Count; cu++)
            {
                SectionInfo si = Info[cu];
                br.BaseStream.Pointer = (StringTables[0] + si.NameIndex);
                string seg_name = "";
                while (true)
                {
                    byte b = br.ReadByte();

                    if (b != 0)
                        seg_name += ((char)b).ToString();
                    else
                        break;
                }
                Info[cu].Name = seg_name;
            }
            for (int cs = 0; cs < Info.Count; cs++)
            {
                SectionInfo si = Info[cs];
                if (si.Name == ".text" && si.Type == 1)
                {
                    SectionData sd = new SectionData();
                    sd.Name = si.Name;
                    br.BaseStream.Pointer = si.Offset;
                    sd.data = new byte[si.Size];
                    sd.LoadAddress = si.Address;
                    for (int i = 0; i < (uint)si.Size; i++)
                    {
                        sd.data[i] = br.ReadByte();
                    }
                    sectionData.Add(sd);
                }
                else if (si.Name == ".symtab")
                {
                    br.BaseStream.Pointer = si.Offset;
                    int total = sizeof(Elf32_sym);
                    uint epos = 0;
                    Elf32_sym* symbol = (Elf32_sym*)symbol_buff;

                    while (si.Size > epos)
                    {
                        for (int i = 0; i < sizeof(Elf32_sym); i++)
                        {
                            symbol_buff[i] = br.ReadByte();
                        }
                        SymbolInfo sy = new SymbolInfo();
                        sy.Value = symbol->st_value;

                        sy.Size = symbol->st_size;
                        sy.Name = getSymbolName(symbol->st_name);
                        sy.Type = symbol->st_info;
                        sy.Other = symbol->st_other;
                        sy.NameIndx = symbol->st_name;
                        Symbols.Add(sy);
                        epos += (uint)sizeof(Elf32_sym);
                    }

                }
                else if (si.Name == ".rel.text")
                {
                    br.BaseStream.Pointer= si.Offset;
                    int total = sizeof(Elf32_Rel);
                    uint epos = 0;
                    Elf32_Rel* symbol = (Elf32_Rel*)rel_buff;
                    while (si.Size > epos)
                    {
                        for (int i = 0; i < sizeof(Elf32_Rel); i++)
                        {
                            rel_buff[i] = br.ReadByte();
                        }
                        epos += (uint)sizeof(Elf32_Rel);
                        symbol = (Elf32_Rel*)rel_buff;
                        RelocInfo ri = new RelocInfo();
                        ri.Address = symbol->r_offset;
                        ri.Symbol = symbol->r_info;
                        relocInfo.Add(ri);
                    }
                }
                else if (si.Name == ".rel.eh_frame")
                {
                    br.BaseStream.Pointer= si.Offset;
                    int total = sizeof(Elf32_Rel);
                    uint epos = 0;
                    Elf32_Rel* symbol = (Elf32_Rel*)rel_buff;
                    while (si.Size > epos)
                    {
                        for (int i = 0; i < sizeof(Elf32_Rel); i++)
                        {
                            rel_buff[i] = br.ReadByte();
                        }
                        epos += (uint)sizeof(Elf32_Rel);
                        symbol = (Elf32_Rel*)rel_buff;
                        RelocInfo ri = new RelocInfo();
                        ri.Address = symbol->r_offset;
                        ri.Symbol = symbol->r_info;
                        relocInfo.Add(ri);
                    }
                }


                else if (si.Name == ".data")
                {
                    SectionData sd = new SectionData();
                    sd.Name = si.Name;
                    br.BaseStream.Pointer = si.Offset;
                    sd.data = new byte[si.Size];
                    sd.LoadAddress = si.Address;

                    for (int i = 0; i < (uint)si.Size; i++)
                    {
                        sd.data[i] = br.ReadByte();
                    }
                    sectionData.Add(sd);
                }
                else if (si.Name.Length > 3)
                {
                    if (si.Name[0] == (byte)'.' && si.Name[1] == (byte)'r' && si.Name[2] == (byte)'o')
                    {
                        SectionData sd = new SectionData();
                        sd.Name = si.Name;
                        br.BaseStream.Pointer = si.Offset;
                        sd.data = new byte[si.Size];
                        sd.LoadAddress = si.Address;
                        for (int i = 0; i < (uint)si.Size; i++)
                        {
                            sd.data[i] = br.ReadByte();
                        }
                        sectionData.Add(sd);

                    }
                }
                //Relocate(0);
            }
            rodata = rd.ToArray();
        }
    }
}