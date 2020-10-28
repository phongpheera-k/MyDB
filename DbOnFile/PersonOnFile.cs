using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using MyClassLibrary.CommonServices;
using MyClassLibrary.Extensions;
using MyClassLibrary.Model;

namespace DbOnFile
{
    public class PersonOnFile
    {
        Encoding Encode { get; set; }
        int NameSize { get; set; } = 255;
        
        int BlockSize { get; set; }

        Dictionary<Encoding, int> EncodeByte = new Dictionary<Encoding, int>()
        {
            {Encoding.ASCII, 1},
            {Encoding.Unicode, 2}
        };
        
        public string DbFileName { get; set; } = "Person.db";
        public string IdIndexFileName { get; set; } = "Id.Index";
        public string NameIndexFileName { get; set; } = "Name.Index";
        public string AgeIndexFileName { get; set; } = "Age.Index";
        public string Folder { get; set; } = "File";

        FileStream DbStream;
        IndexOnFile<int> idIndex = new IndexOnFile<int>(CompareService.CompareInt, ConvertService.ConvertInt, ConvertService.TransformInt);
        IndexOnFile<string> nameIndex = new IndexOnFile<string>(CompareService.CompareString, ConvertService.ConvertString, ConvertService.TransfromString);
        IndexOnFile<int> ageIndex = new IndexOnFile<int>(CompareService.CompareInt, ConvertService.ConvertInt, ConvertService.TransformInt);

        public PersonOnFile(Encoding encoding)
        {
            Encode = encoding;
            Directory.CreateDirectory(Folder);
            DbStream = File.Open(Path.Combine(Folder, DbFileName), FileMode.OpenOrCreate);
            BlockSize = GetBlockSize();
            idIndex.SetIndex(Folder, IdIndexFileName, sizeof(int), Encode);
            nameIndex.SetIndex(Folder, NameIndexFileName, NameSize * EncodeByte[Encode], Encode);
            ageIndex.SetIndex(Folder, AgeIndexFileName, sizeof(int), Encode);
        }

        public PersonOnFile(Encoding encoding, int nameSize) : this(encoding)
        {
            NameSize = nameSize;
        }

        public void Disconnect()
        {
            DbStream.Close();
        }

        byte[] ToBytes(Person person)
        {
            return (new []
            {
                person.Id.ToBytes(),
                person.Name.PadRight(NameSize).ToBytes(Encode),
                person.Age.ToBytes()
            }).Combine();
        }
        
        int GetBlockSize() => sizeof(int) //Id
                   + (EncodeByte[Encode] * NameSize) //Name
                   + sizeof(int); //Age

        int Length => (int) (DbStream.Length / BlockSize);

        int GetFileOffset(int num) => num * BlockSize;

        Person ToPerson(byte[] data)
        {
            var idBytes = data.Seperate(0, sizeof(int));
            var nameBytes = data.Seperate(sizeof(int), EncodeByte[Encode] * NameSize);
            var ageBytes = data.Seperate(BlockSize - sizeof(int), sizeof(int));

            var id = idBytes.ToInt32();
            var name = nameBytes.ToStringWithEncoding(Encode).TrimEnd();
            var age = ageBytes.ToInt32();

            return new Person(id, name, age);
        }

        public void InsertData(Person person)
        {
            var data = ToBytes(person);

            DbStream.Seek(0, SeekOrigin.End);
            DbStream.Write(data, 0, BlockSize);
            
            //--indexing--
            var valueOrder = (int)(DbStream.Position / BlockSize);
            idIndex.Indexing(person.Id, valueOrder);
            nameIndex.Indexing(person.Name, valueOrder);
            ageIndex.Indexing(person.Age, valueOrder);
        }

        public Person SelectData(int num)
        {
            if (num > Length)
            {
                throw new ArgumentOutOfRangeException();
            }

            var fileOffset = GetFileOffset(num);
            DbStream.Seek(fileOffset, SeekOrigin.Begin);
            
            var data = new byte[BlockSize];
            DbStream.Read(data, 0, BlockSize);
            return ToPerson(data);
        }
        
        int GetIdInDb(int num)
        {
            var fileOffset = GetFileOffset(num);
            DbStream.Seek(fileOffset, SeekOrigin.Begin);
            
            var idBytes = new byte[sizeof(int)];
            DbStream.Read(idBytes, 0, sizeof(int));
            return idBytes.ToInt32();
        }

        public Person[] SelectAllData()
        {
            var records = Length;
            var persons = new Person[records];
            for (int num = 0; num < records; num++)
            {
                persons[num] = SelectData(num);
            }
            
            return persons;
        }

        public Person? SelectById(int id)
        {
            return SearchByIdOrderList(id);
        }

        Person? SearchByIdOrderList(int id)
        {
            var result = -1;
            var record = 0;
            while (result < 0 && record < Length)
            {
                var idTemp = GetIdInDb(record);
                if (id == idTemp)
                {
                    result = record;
                }
                record++;
            }

            return result >= 0 ? SelectData(result) : null;
        }

        public Person SearchIndexById(int id)
        {
            var nums = idIndex.SearchNode(id);
            return SelectData(nums[0]);
        }

        public Person[] SearchIndexByName(string name)
        {
            var nums = nameIndex.SearchNode(name);
            var persons = new Person[nums.Length];
            for (int i = 0; i < nums.Length; i++)
            {
                persons[i] = SelectData(nums[i]);
            }
            return persons;
        }

        public Person[] SearchIndexByAge(int age)
        {
            var nums = ageIndex.SearchNode(age);
            var persons = new Person[nums.Length];
            for (int i = 0; i < nums.Length; i++)
            {
                persons[i] = SelectData(nums[i]);
            }
            return persons;
        }
    }
}