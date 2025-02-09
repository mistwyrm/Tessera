using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Reactive.Linq;
using System.Threading.Tasks;
using Avalonia.Platform.Storage;
using MySqlConnector;
using Tessera.Models;

namespace Tessera.Services
{
    public class ImportFile
    {
        public string path { get; set; }
        public string name { get; set; }

        public ImportFile(string path, string name)
        {
            this.path = path;
            this.name = name;
        }
    }

    public class FileEntityRelationsService : IFileEntityRelationsService
    {
        /// <summary>
        /// Assigns given files to a given entity
        /// </summary>
        /// <param name="entityId"></param>
        /// <param name="Files"></param>
        /// <returns></returns>
        public bool AddFilesToEntity(int? entityId, IEnumerable<FileObject> files)
        {
            foreach (FileObject file in files)
            {
                sqlConnection test = new sqlConnection();
                MySqlConnection connection = test.GetSqlConnection();
                connection.Open();

                using var cmd = new MySqlCommand("CALL entity_api.addFileToEntity(@entityid, @fileid)", connection);
                cmd.Parameters.AddWithValue("entityid", entityId);
                cmd.Parameters.AddWithValue("fileid", file.Id);
                cmd.ExecuteNonQuery();
            }

            return true;
        }

        /// <summary>
        /// Adds a given tag to a given entity
        /// </summary>
        /// <param name="id"></param>
        /// <param name="newTag"></param>
        public async void AddTagToEntity(int id, string newTag)
        {
            sqlConnection test = new sqlConnection();
            MySqlConnection connection = test.GetSqlConnection();
            await connection.OpenAsync();
            using var cmd = new MySqlCommand("CALL entity_api.addTagToEntity(@id, @tag)", connection);
            cmd.Parameters.AddWithValue("tag", newTag);
            cmd.Parameters.AddWithValue("id", id);
            await cmd.ExecuteNonQueryAsync();
        }

        /// <summary>
        /// Creates a new entity with a given name
        /// </summary>
        /// <param name="name"></param>
        public async void CreateEntity(string name)
        {
            sqlConnection test = new sqlConnection();
            MySqlConnection connection = test.GetSqlConnection();
            await connection.OpenAsync();
            using var cmd = new MySqlCommand("CALL entity_api.addEntity(@name)", connection);
            cmd.Parameters.AddWithValue("name", name);
            await cmd.ExecuteNonQueryAsync();
        }

        /// <summary>
        /// Returns a collection of all entities
        /// </summary>
        /// <returns></returns>
        public ObservableCollection<EntityObject> GetEntities()
        {
            sqlConnection test = new sqlConnection();
            MySqlConnection connection = test.GetSqlConnection();
            connection.Open();
            using var cmd = new MySqlCommand("CALL entity_api.getEntities()", connection);
            using var reader = cmd.ExecuteReader();

            ObservableCollection<EntityObject> Entities = new ObservableCollection<EntityObject>();

            while (reader.Read())
            {
                EntityObject entity = new EntityObject
                {
                    Id = reader.GetInt32("entity_id"),
                    Name = reader.GetString("entity_name"),
                    IsGhost = reader.GetByte("entity_is_ghost") == 1,
                };
                if(!reader.IsDBNull(reader.GetOrdinal("tags")))
                {
                    entity.Tags = new ObservableCollection<string>(reader.GetString("tags").Split(","));
                }
                Entities.Add(entity);
            }

            return Entities;
        }

        /// <summary>
        /// Returns an entity object given an entity id
        /// </summary>
        /// <param name="entityId"></param>
        /// <returns></returns>
        public EntityObject GetEntityFromId(int entityId)
        {
            sqlConnection test = new sqlConnection();
            MySqlConnection connection = test.GetSqlConnection();
            connection.Open();
            using var cmd = new MySqlCommand("CALL entity_api.getEntityFromId(@entityid)", connection);
            cmd.Parameters.AddWithValue("entityid", entityId);
            using var reader = cmd.ExecuteReader();
            reader.Read();
            EntityObject entity = new EntityObject
            {
                Id = reader.GetInt32("entity_id"),
                Name = reader.GetString("entity_name"),
                IsGhost = reader.GetByte("entity_is_ghost") == 1,
            };

            return entity;
        }

        /// <summary>
        /// Returns a collection files in the given entity's id
        /// </summary>
        /// <param name="entityId"></param>
        /// <returns></returns>
        public ObservableCollection<FileObject> GetFiles(int? entityId)
        {
            sqlConnection test = new sqlConnection();
            MySqlConnection connection = test.GetSqlConnection();
            connection.Open();

            using var cmd = new MySqlCommand("CALL file_api.getFiles(true, @entityid)", connection);
            cmd.Parameters.AddWithValue("entityid", entityId);
            using var reader = cmd.ExecuteReader();

            ObservableCollection<FileObject> Files = new ObservableCollection<FileObject>();

            while (reader.Read())
            {
                FileObject file = new FileObject
                {
                    Id = reader.GetInt32("file_id"),
                    Name = reader.GetString("file_name"),
                    Path = reader.GetString("file_address"),
                };
                Files.Add(file);
            }

            return Files;
        }

        /// <summary>
        /// Extracts file details and adds new file objects to the database 
        /// </summary>
        /// <param name="files"></param>
        /// <param name="entityId"></param>
        /// <returns></returns>
        async public Task<bool> ImportFiles(IReadOnlyList<IStorageFile> files, int? entityId)
        {
            if (files.Count == 0)
            {
                return false;
            }

            List<ImportFile> FilePaths = new List<ImportFile>();
            foreach (IStorageFile file in files)
            {
                FilePaths.Add(new ImportFile(file.TryGetLocalPath(), file.Name));
            }

            return await ImportFiles(FilePaths, entityId);
        }

        /// <summary>
        /// Extracts file details and adds new file objects to the database 
        /// </summary>
        /// <param name="files"></param>
        /// <param name="entityId"></param>
        /// <returns></returns>
        async public Task<bool> ImportFiles(IEnumerable<IStorageItem> files, int? entityId)
        {
            if (!files.Any())
            {
                return false;
            }

            List<ImportFile> FilePaths = new List<ImportFile>();
            foreach (IStorageItem file in files)
            {
                FilePaths.Add(new ImportFile(file.Path.LocalPath, file.Name));
            }

            return await ImportFiles(FilePaths, entityId);
        }

        /// <summary>
        /// Updates the database with changes to the local entity
        /// </summary>
        /// <param name="entity"></param>
        async public void UpdateEntity(EntityObject entity)
        {
            sqlConnection test = new sqlConnection();
            MySqlConnection connection = test.GetSqlConnection();
            await connection.OpenAsync();
            using var cmd = new MySqlCommand("CALL entity_api.updateEntity(@id, @name)", connection);
            cmd.Parameters.AddWithValue("name", entity.Name);
            cmd.Parameters.AddWithValue("id", entity.Id);
            await cmd.ExecuteNonQueryAsync();
        }

        /// <summary>
        /// Updates the database with changes to the local file
        /// </summary>
        /// <param name="file"></param>
        async public void UpdateFile(FileObject file)
        {
            sqlConnection test = new sqlConnection();
            MySqlConnection connection = test.GetSqlConnection();
            await connection.OpenAsync();
            using var cmd = new MySqlCommand("CALL file_api.updateFile(@id, @name)", connection);
            cmd.Parameters.AddWithValue("name", file.Name);
            cmd.Parameters.AddWithValue("id", file.Id);
            await cmd.ExecuteNonQueryAsync();
        }


        /// <summary>
        /// Imports a given list of file details. Optional entity id can be given to automatically add all files to entity
        /// </summary>
        /// <param name="files"></param>
        /// <param name="entityId"></param>
        /// <returns></returns>
        async private Task<bool> ImportFiles(List<ImportFile> files, int? entityId)
        {
            sqlConnection test = new sqlConnection();
            MySqlConnection connection = test.GetSqlConnection();
            await connection.OpenAsync();

            using var batch = new MySqlBatch(connection);

            foreach (ImportFile file in files)
            {
                MySqlBatchCommand cmd = new MySqlBatchCommand("CALL file_api.importFile(@name, @address, @entityid)");
                cmd.Parameters.AddWithValue("name", file.name);
                cmd.Parameters.AddWithValue("address", file.path);
                cmd.Parameters.AddWithValue("entityid", entityId);
                batch.BatchCommands.Add(cmd);
            }
            await batch.ExecuteNonQueryAsync();

            return true;
        }
    }
}
