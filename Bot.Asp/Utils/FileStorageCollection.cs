using ConsoleApp3.Entities.Base;
using Newtonsoft.Json;

namespace ConsoleApp3
{
    public class FileStorageCollection<T> : List<T>
        where T : BaseEntity
    {
        public FileInfo FileInfo { get; init; }
        public FileStorageCollection(string fielPath, bool restore = false)
        {
            FileInfo = new FileInfo(fielPath);

            if (restore)
                Restore();
        }

        public new void Add(T item)
        {
            if (!this.Any(x => x.Id == item.Id))
            {
                base.Add(item);
            }
        }

        private void Restore()
        {
            if (!FileInfo.Exists)
            {
                if (!FileInfo.Directory.Exists)
                {
                    FileInfo.Directory.Create();
                }

                using var sw = FileInfo.CreateText();
            }
            using FileStream stream = FileInfo.OpenRead();

            using var sr = new StreamReader(stream);

            while (!sr.EndOfStream)
            {
                string json = sr.ReadLine()!;

                T obj = JsonConvert.DeserializeObject<T>(json)!;

                if (obj is not null)
                {
                    Add(obj);
                }
            }
        }
        public void Save()
        {
            using FileStream stream = FileInfo.Open(FileMode.OpenOrCreate);

            using var sw = new StreamWriter(stream);

            foreach (var data in this)
            {
                sw.WriteLine(JsonConvert.SerializeObject(data));
            }
        }
    }
}
