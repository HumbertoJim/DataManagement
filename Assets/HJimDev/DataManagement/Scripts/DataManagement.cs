using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Runtime.Serialization.Formatters.Binary;
using System.IO;
using System;

namespace DataManagement
{
    namespace Managers
    {
        public class BaseManager : MonoBehaviour
        {
            protected Serializers.BaseSerializer Serializer { get; set; }

            protected virtual void Awake() { throw new NotImplementedException(); }

            public virtual void SaveData() { Serializer.SaveData(); }

            public virtual void ResetData() { Serializer.ResetData(); }

            public virtual void SaveReset() { Serializer.SaveReset(); }
        }

        public class NamedManager : BaseManager
        {
            [Header("Name")]
            [SerializeField] protected string dataName = "Rename";
        }

        public class VariableManager : NamedManager
        {
            [Header("Validation File")]

            [SerializeField] private TextAsset file;

            protected new Serializers.ValidationExtension.VariableSerializer Serializer
            {
                get { return (Serializers.ValidationExtension.VariableSerializer)base.Serializer; }
                set { base.Serializer = value; }
            }

            protected sealed override void Awake()
            {
                Serializer = new Serializers.ValidationExtension.VariableSerializer(dataName + "Variable", ValidatedValue());
                Serializer.Initialize();
            }

            protected string ValidatedValue()
            {
                return file.ToString().Trim();
            }
        }

        public class DictionaryManager : NamedManager
        {
            [Header("Validation File")]

            [SerializeField] private TextAsset file;
            [SerializeField] private string equalKey = ":";

            protected new Serializers.ValidationExtension.DictionarySerializer Serializer
            {
                get { return (Serializers.ValidationExtension.DictionarySerializer)base.Serializer; }
                set { base.Serializer = value; }
            }

            protected sealed override void Awake()
            {
                Serializer = new Serializers.ValidationExtension.DictionarySerializer(dataName + "Dictionary", ValidatedElements());
                Serializer.Initialize();
            }

            protected Dictionary<string, string> ValidatedElements()
            {
                Dictionary<string, string> validated_elements = new Dictionary<string, string>();
                string[] lines = file.ToString().Replace('\r', '\n').Split('\n');
                string line, key;
                foreach (string _line_ in lines)
                {
                    line = _line_.Trim();
                    if (line != "" && line[0] != '#' && line.Contains(equalKey))
                    {
                        key = line.Split(new string[] { equalKey }, StringSplitOptions.None)[0];
                        validated_elements.Add(key.Trim(), line.Substring(key.Length + equalKey.Length).Trim());
                    }
                }
                return validated_elements;
            }
        }

        public class BooleanDictionaryManager : NamedManager
        {
            [Header("Validation File")]

            [SerializeField] private TextAsset file;

            protected new Serializers.ValidationExtension.DictionarySerializer Serializer
            {
                get { return (Serializers.ValidationExtension.DictionarySerializer)base.Serializer; }
                set { base.Serializer = value; }
            }

            protected sealed override void Awake()
            {
                Serializer = new Serializers.ValidationExtension.DictionarySerializer(dataName + "BooleanDictionary", ValidatedElements());
                Serializer.Initialize();
            }

            protected Dictionary<string, string> ValidatedElements()
            {
                Dictionary<string, string> validated_elements = new Dictionary<string, string>();
                string[] lines = file.ToString().Replace('\r', '\n').Split('\n');
                string line;
                foreach (string _line_ in lines)
                {
                    line = _line_.Trim();
                    if (line != "" && line[0] != '#') validated_elements.Add(line, "false");
                }
                return validated_elements;
            }
        }

        public class TableManager : NamedManager
        {
            [Header("Validation File")]

            [Tooltip("A file containing a list of allowed rows")]
            [SerializeField] private TextAsset rows;
            [Tooltip("A file containing a list of fields with default values")]
            [SerializeField] private TextAsset fields;
            [SerializeField] private string equalKey = ":";

            protected new Serializers.ValidationExtension.TableSerializer Serializer
            {
                get { return (Serializers.ValidationExtension.TableSerializer)base.Serializer; }
                set { base.Serializer = value; }
            }

            protected sealed override void Awake()
            {
                Serializer = new Serializers.ValidationExtension.TableSerializer(dataName + "Table", ValidatedFields(), ValidatedRows());
                Serializer.Initialize();
            }

            protected Dictionary<string, string> ValidatedFields()
            {
                Dictionary<string, string> validated_fields = new Dictionary<string, string>();
                string[] lines = fields.ToString().Replace('\r', '\n').Split('\n');
                string line, key;
                foreach (string _line_ in lines)
                {
                    line = _line_.Trim();
                    if (line != "" && line[0] != '#' && line.Contains(equalKey))
                    {
                        key = line.Split(new string[] { equalKey }, StringSplitOptions.None)[0];
                        validated_fields.Add(key.Trim(), line.Substring(key.Length + equalKey.Length).Trim());
                    }
                }
                return validated_fields;
            }

            List<string> ValidatedRows()
            {
                List<string> validated_rows = new List<string>();
                string[] lines = fields.ToString().Replace('\r', '\n').Split('\n');
                foreach (string line in lines)
                {
                    validated_rows.Add(line.Trim());
                }
                return validated_rows;
            }
        }

        public class DictionaryCollectionManager : NamedManager
        {
            [Header("Validation File")]
            [SerializeField] private TextAsset[] files;
            [SerializeField] private string equalKey = ":";

            protected new Serializers.ValidationExtension.DictionaryCollectionSerializer Serializer
            {
                get { return (Serializers.ValidationExtension.DictionaryCollectionSerializer)base.Serializer; }
                set { base.Serializer = value; }
            }

            protected sealed override void Awake()
            {
                Serializer = new Serializers.ValidationExtension.DictionaryCollectionSerializer(dataName + "DictionaryCollection", ValidatedDictionaries());
                Serializer.Initialize();
            }

            protected Dictionary<string, Dictionary<string, string>> ValidatedDictionaries()
            {
                Dictionary<string, Dictionary<string, string>> dictionaries = new Dictionary<string, Dictionary<string, string>>();
                string[] lines;
                string line, key;
                foreach (TextAsset file in files)
                {
                    dictionaries.Add(file.name, new Dictionary<string, string>());
                    lines = file.ToString().Replace('\r', '\n').Split('\n');
                    foreach (string _line_ in lines)
                    {
                        line = _line_.Trim();
                        if (line != "" && line[0] != '#' && line.Contains(equalKey))
                        {
                            key = line.Split(new string[] { equalKey }, StringSplitOptions.None)[0];
                            dictionaries[file.name].Add(key.Trim(), line.Substring(key.Length + equalKey.Length).Trim());
                        }
                    }
                }
                return dictionaries;
            }
        }
    }

    namespace Serializers
    {
        public class BaseSerializer
        {
            protected virtual void CheckDataConsistensy() { throw new NotImplementedException(); }

            public virtual void SaveData() { throw new NotImplementedException(); }

            public virtual void ResetData() { throw new NotImplementedException(); }

            public void SaveReset() { ResetData(); CheckDataConsistensy(); SaveData(); }
        }

        public class BaseSerializer<T> : BaseSerializer
        {
            private Structs.CoreAttribute<string> name;
            private Structs.CoreAttribute<string> filePath;
            private BinaryFormatter bf;
            protected T data;


            public BaseSerializer(string name)
            {
                this.name.Value = name;
            }

            public void Initialize()
            {
                filePath.Value = Application.persistentDataPath + "/" + name + "Data.dat";
                bf = new BinaryFormatter();
                if (!File.Exists(filePath.Value))
                {
                    Debug.Log("A new " + name + "Data file will be made");
                    ResetData();
                    SaveData();
                }
                else data = Deserialize();
                CheckDataConsistensy();
                SaveData();
            }

            public sealed override void SaveData()
            {
                Serialize(data);
            }

            protected void ResetData(T data)
            {
                FileStream file = File.Create(filePath.Value);
                file.Close();
                this.data = data;
            }

            private T Deserialize()
            {
                FileStream file = File.Open(filePath.Value, FileMode.Open);
                T data = (T)bf.Deserialize(file);
                file.Close();
                return data;
            }

            private void Serialize(T data)
            {
                FileStream file = File.Open(filePath.Value, FileMode.Open);
                bf.Serialize(file, data);
                file.Close();
            }
        }

        public class VariableSerializer : BaseSerializer<Serializable.Variable>
        {
            public VariableSerializer(string name) : base(name) { }

            protected override void CheckDataConsistensy()
            {
                data.ValidateData();
            }

            public sealed override void ResetData()
            {
                data = new Serializable.Variable();
                ResetData(data);
            }

            public void Set(string value)
            {
                data.Set(value);
            }

            public string Get()
            {
                return data.Get();
            }

            public void SetAsInt(string value)
            {
                try
                {
                    int.Parse(value);
                    Set(value);
                }
                catch
                {
                    Debug.Log("Unable to save data because can not parse to int: " + value);
                }
            }

            public int GetAsInt()
            {
                return int.Parse(Get());
            }

            public void SetAsBool(string value)
            {
                Set(value.Trim().ToLower() == "true" ? "true" : "false");
            }

            public bool GetAsBool()
            {
                return Get() == "true";
            }
        }

        public class DictionarySerializer : BaseSerializer<Serializable.Dictionary>
        {
            public DictionarySerializer(string name) : base(name) { }

            protected override void CheckDataConsistensy()
            {
                data.ValidateData();
            }

            public sealed override void ResetData()
            {
                data = new Serializable.Dictionary();
                ResetData(data);
            }

            public List<string> GetKeys()
            {
                return data.GetKeys();
            }

            public bool DataExists(string id)
            {
                return data.DataExists(id);
            }

            public void SetData(string id, string value)
            {
                data.SetData(id, value);
            }

            public string GetData(string id)
            {
                return data.GetData(id);
            }

            public void SetDataAsInt(string id, string value)
            {
                try
                {
                    int.Parse(value);
                    SetData(id, value);
                }
                catch
                {
                    Debug.Log("Unable to save data because can not parse to int: " + id + " = " + value);
                }
            }

            public int GetDataAsInt(string id)
            {
                return int.Parse(GetData(id));
            }

            public void SetDataAsBool(string id, string value)
            {
                SetData(id, value.Trim().ToLower() == "true" ? "true" : "false");
            }

            public bool GetDataAsBool(string id)
            {
                return GetData(id) == "true";
            }
        }

        public class TableSerializer : BaseSerializer<Serializable.Table>
        {
            private readonly Dictionary<string, string> fields;

            public TableSerializer(string name, Dictionary<string, string> fields) : base(name)
            {
                this.fields = new Dictionary<string, string>(fields);
            }

            public sealed override void ResetData()
            {
                data = new Serializable.Table();
                ResetData(data);
            }

            protected override void CheckDataConsistensy()
            {
                data.ValidateData(fields);
            }

            public bool FieldExists(string field)
            {
                return data.FieldExists(field);
            }

            public List<string> GetFields()
            {
                return data.GetFields();
            }

            public List<string> GetRows()
            {
                return data.GetRows();
            }

            public bool RowExists(string row)
            {
                return data.RowExists(row);
            }

            public void SetData(string row, string field, string value)
            {
                data.SetData(row, field, value);
            }

            public string GetData(string row, string field)
            {
                return data.GetData(row, field);
            }

            public void SetDataAsInt(string row, string field, string value)
            {
                try
                {
                    int.Parse(value);
                    SetData(row, field, value);
                }
                catch
                {
                    Debug.Log("Unable to save data because can not parse to int: " + row + " = " + value);
                }
            }

            public int GetDataAsInt(string row, string field)
            {
                return int.Parse(GetData(row, field));
            }

            public void SetDataAsBool(string row, string field, string value)
            {
                SetData(row, field, value.Trim().ToLower() == "true" ? "true" : "false");
            }

            public bool GetDataAsBool(string row, string field)
            {
                return GetData(row, field) == "true";
            }
        }

        public class DictionaryCollectionSerializer : BaseSerializer<Serializable.DictionaryCollection>
        {
            private readonly List<string> dictionaries;

            public DictionaryCollectionSerializer(string name, List<string> dictionaries) : base(name)
            {
                this.dictionaries = new List<string>(dictionaries);
            }

            public sealed override void ResetData()
            {
                data = new Serializable.DictionaryCollection();
                ResetData(data);
            }

            protected override void CheckDataConsistensy()
            {
                data.ValidateData(dictionaries);
            }

            public bool DictionaryExists(string dictionary)
            {
                return data.DictionaryExists(dictionary);
            }

            public bool DataExists(string dictionary, string id)
            {
                return data.DataExists(dictionary, id);
            }

            public List<string> GetDictionaries()
            {
                return data.GetDictionaries();
            }

            public List<string> GetKeys(string dictionary)
            {
                return data.GetKeys(dictionary);
            }

            public void SetData(string dictionary, string id, string value)
            {
                data.SetData(dictionary, id, value);
            }

            public string GetData(string dictionary, string id)
            {
                return data.GetData(dictionary, id);
            }

            public void SetDataAsInt(string dictionary, string id, string value)
            {
                try
                {
                    int.Parse(value);
                    SetData(dictionary, id, value);
                }
                catch
                {
                    Debug.Log("Unable to save data because can not parse to int: " + id + " = " + value);
                }
            }

            public int GetDataAsInt(string dictionary, string id)
            {
                return int.Parse(GetData(dictionary, id));
            }

            public void SetDataAsBool(string dictionary, string id, string value)
            {
                SetData(dictionary, id, value.Trim().ToLower() == "true" ? "true" : "false");
            }

            public bool GetDataAsBool(string dictionary, string id)
            {
                return GetData(dictionary, id) == "true";
            }
        }

        namespace ValidationExtension
        {
            public class VariableSerializer : Serializers.VariableSerializer
            {
                private readonly string validated_value;

                public VariableSerializer(string name, string validated_value) : base(name)
                {
                    this.validated_value = validated_value;
                }

                protected sealed override void CheckDataConsistensy()
                {
                    base.CheckDataConsistensy();

                    if(data.Get() == "")
                    {
                        data.Set(validated_value);
                    }
                }
            }

            public class DictionarySerializer : Serializers.DictionarySerializer
            {
                private readonly Dictionary<string, string> validated_elements;

                public DictionarySerializer(string name, Dictionary<string, string> validated_elements) : base(name)
                {
                    this.validated_elements = validated_elements;
                }

                protected sealed override void CheckDataConsistensy()
                {
                    base.CheckDataConsistensy();

                    // remove keys that does not belong to validated data
                    List<string> currentKeys = data.GetKeys();
                    foreach (string key in currentKeys)
                    {
                        if (!validated_elements.ContainsKey(key)) data.RemoveData(key);
                    }

                    // add new keys to data
                    foreach (string element in validated_elements.Keys)
                    {
                        if (!data.DataExists(element)) data.SetData(element, validated_elements[element]);
                    }
                }
            }

            public class TableSerializer : Serializers.TableSerializer
            {
                private readonly List<string> validated_rows;

                public TableSerializer(string name, Dictionary<string, string> validated_fields, List<string> validated_rows) : base(name, validated_fields)
                {
                    this.validated_rows = validated_rows;
                }

                protected sealed override void CheckDataConsistensy()
                {
                    base.CheckDataConsistensy();

                    // remove rows that does not belong to validated data
                    List<string> rows = data.GetRows();
                    foreach (string row in rows)
                    {
                        if (!validated_rows.Contains(row)) data.RemoveRow(row);
                    }

                    // add new rows to data
                    foreach (string row in validated_rows)
                    {
                        if (!data.RowExists(row)) data.SetRow(row);
                    }
                }
            }

            public class DictionaryCollectionSerializer : Serializers.DictionaryCollectionSerializer
            {
                private readonly Dictionary<string, Dictionary<string, string>> validated_dictionaries;

                public DictionaryCollectionSerializer(string name, Dictionary<string, Dictionary<string, string>> validated_dictionaries) : base(name, new List<string>(validated_dictionaries.Keys))
                {
                    this.validated_dictionaries = validated_dictionaries;
                }

                protected sealed override void CheckDataConsistensy()
                {
                    base.CheckDataConsistensy();

                    foreach (string dictionary in validated_dictionaries.Keys)
                    {
                        // remove rows that does not belong to validated data
                        List<string> elements = new List<string>(data.GetDictionary(dictionary).Keys);
                        foreach (string element in elements)
                        {
                            if (!validated_dictionaries[dictionary].ContainsKey(element)) data.RemoveData(dictionary, element);
                        }

                        // add new rows to data
                        foreach (string element in validated_dictionaries[dictionary].Keys)
                        {
                            if (!data.DataExists(dictionary, element)) data.SetData(dictionary, element, validated_dictionaries[dictionary][element]);
                        }
                    }
                }
            }
        }
    }

    namespace Serializable
    {
        [Serializable]
        public class Variable
        {
            public string data;

            public void ValidateData()
            {
                if (data == null) data = "";
            }

            public void Set(string value)
            {
                data = value;
            }

            public string Get()
            {
                return data;
            }
        }

        [Serializable]
        public class Dictionary
        {
            public Dictionary<string, string> data;

            public void ValidateData()
            {
                if (data == null) data = new Dictionary<string, string>();
            }

            public bool DataExists(string key)
            {
                return data.ContainsKey(key);
            }

            public string GetData(string key)
            {
                return data[key];
            }

            public void SetData(string key, string value)
            {
                data[key] = value;
            }

            public void RemoveData(string key)
            {
                data.Remove(key);
            }

            public List<string> GetKeys()
            {
                return new List<string>(data.Keys);
            }
        }

        public class Table
        {
            Dictionary<string, Dictionary<string, string>> data; // each element represents a row, and a row correspond to a field (field-value)
            Dictionary<string, string> fields; // each element represents a dictionaries with its defaul value

            public void ValidateData()
            {
                if (fields == null) fields = new Dictionary<string, string>();
                if (data == null) data = new Dictionary<string, Dictionary<string, string>>();
                foreach (string row in data.Keys)
                {
                    List<string> rowDictionarys = new List<string>(data[row].Keys);
                    foreach (string field in rowDictionarys)
                    {
                        if (!fields.ContainsKey(field)) data[row].Remove(field);
                    }

                    foreach (string field in fields.Keys)
                    {
                        if (!data[row].ContainsKey(field)) data[row].Add(field, fields[field]);
                    }
                }
            }

            public void ValidateData(Dictionary<string, string> fields)
            {
                this.fields = fields;
                ValidateData();
            }

            public bool FieldExists(string field)
            {
                return fields.ContainsKey(field);
            }

            public bool RowExists(string row)
            {
                return data.ContainsKey(row);
            }

            public void SetRow(string row)
            {
                Dictionary<string, string> new_data = new Dictionary<string, string>();
                foreach (string field in fields.Keys)
                {
                    new_data.Add(field, fields[field]);
                }
                data[row] = new_data;
            }

            public void SetRow(string row, Dictionary<string, string> data)
            {
                Dictionary<string, string> new_data = new Dictionary<string, string>();
                Dictionary<string, string> defaultFields = this.data.ContainsKey(row) ? this.data[row] : fields;
                foreach (string field in defaultFields.Keys)
                {
                    new_data.Add(field, data.ContainsKey(field) ? data[field] : defaultFields[field]);
                }
                this.data[row] = new_data;
            }

            public Dictionary<string, string> GetRow(string row)
            {
                return data[row];
            }

            public void RemoveRow(string row)
            {
                data.Remove(row);
            }

            public List<string> GetRows()
            {
                return new List<string>(data.Keys);
            }

            public List<string> GetFields()
            {
                return new List<string>(fields.Keys);
            }

            public string GetData(string row, string field)
            {
                return data[row][field];
            }

            public void SetData(string row, string field, string value)
            {
                if (fields.ContainsKey(field)) data[row][field] = value;
            }
        }

        public class DictionaryCollection
        {
            Dictionary<string, Dictionary<string, string>> data; // data[dictionary_id][data_id] = value;
            List<string> dictionaries;

            public void Validate()
            {
                if (data == null) data = new Dictionary<string, Dictionary<string, string>>();
                if (dictionaries == null) dictionaries = new List<string>();

                List<string> currentDictionaries = new List<string>(data.Keys);
                foreach (string dictionary in currentDictionaries)
                {
                    if (!dictionaries.Contains(dictionary)) data.Remove(dictionary);
                }
                foreach (string dictionary in dictionaries)
                {
                    if (!data.ContainsKey(dictionary)) data.Add(dictionary, new Dictionary<string, string>());
                }
            }

            public void ValidateData(List<string> dictionaries)
            {
                this.dictionaries = dictionaries;
                Validate();
            }

            public List<string> GetDictionaries()
            {
                return new List<string>(dictionaries);
            }

            public List<string> GetKeys(string dictionary)
            {
                return new List<string>(data[dictionary].Keys);
            }

            public bool DictionaryExists(string dictionary)
            {
                return data.ContainsKey(dictionary);
            }

            public Dictionary<string, string> GetDictionary(string dictionary)
            {
                return new Dictionary<string, string>(data[dictionary]);
            }

            public void SetDictionary(string dictionary, Dictionary<string, string> value)
            {
                data[dictionary] = value;
            }

            public bool DataExists(string dictionary, string id)
            {
                return data[dictionary].ContainsKey(id);
            }

            public string GetData(string dictionary, string id)
            {
                return data[dictionary][id];
            }

            public void SetData(string dictionary, string id, string value)
            {
                data[dictionary][id] = value;
            }

            public void RemoveData(string dictionary, string id)
            {
                data[dictionary].Remove(id);
            }
        }
    }

    namespace Structs
    {
        public struct CoreAttribute<T>
        {
            private T value;
            private bool isFixed;

            public T Value
            {
                get
                {
                    if (isFixed) return value;
                    else throw new Exceptions.NullAttributeException();
                }
                set
                {
                    if (!isFixed)
                    {
                        this.value = value;
                        isFixed = true;
                    }
                    else throw new Exceptions.MultipleSetAttributeException();
                }
            }

            public override string ToString()
            {
                return Value.ToString();
            }
        }

        [System.Serializable]
        public struct TextAsset
        {
            public string name;
            public UnityEngine.TextAsset file;

            public override string ToString()
            {
                return name;
            }
        }

            
    }

    namespace Exceptions
    {
        public class NullAttributeException : Exception
        {
            public NullAttributeException() : base("Attribute was not fixed.") { }
            public NullAttributeException(string name) : base("Attribute " + name + " was not fixed.") { }
        }
        public class MultipleSetAttributeException : Exception
        {
            public MultipleSetAttributeException() : base("Trying to set Attribute more than once.") { }
            public MultipleSetAttributeException(string name) : base("Trying to set the Attribute " + name + " more than once.") { }
        }
    }
}