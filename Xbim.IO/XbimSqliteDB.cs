using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SQLite;
using System.Linq;
using System.Text;

namespace Xbim.IO
{
    public class XbimSqliteDB : IDisposable
    {
        private string _dataBaseName;

        private string ConnectionString
        {
            get
            {
                return "Data Source=" + _dataBaseName + ";";
            }
        }
        public XbimSqliteDB(string databaseName)
        {
            _dataBaseName = databaseName;
            this.InitStructure();
        }

        public void InitStructure()
        {
            //Create the scene table
            this.ExecuteSQL("CREATE TABLE IF NOT EXISTS 'Scenes' (" +
                    "'SceneId' INTEGER PRIMARY KEY," +
                    "'SceneName' VARCHAR NOT NULL," +
                    "'BoundingBox' BLOB" +
                    ");");
            //create the layer table
            this.ExecuteSQL("CREATE TABLE IF NOT EXISTS 'Layers' (" +
                    "'SceneName' VARCHAR NOT NULL," +
                    "'LayerName' VARCHAR NOT NULL," +
                    "'LayerId' INTEGER PRIMARY KEY," +
                    "'ParentLayerId' INTEGER, " +
                    "'Meshes' BLOB, " +
                    "'XbimTexture' BLOB," +
                    "'BoundingBox' BLOB" +
                    ");");
            //create the meta data table
            this.ExecuteSQL("CREATE TABLE IF NOT EXISTS 'Meta' (" +
                    "'Meta_ID' INTEGER PRIMARY KEY," +
                    "'Meta_type' text not null," +
                    "'Meta_key' text," +
                    "'Meta_intkey' INTEGER," +
                    "'Meta_Value' text not null" +
                    ");");
        }

        /// <summary>
        /// runs the SQL statement no return value.
        /// </summary>
        /// <param name="sql"></param>
        public void ExecuteSQL(string sql)
        {
            Flush();
            using (var mDBCon = this.GetConnection())
            {
                SQLiteCommand cmd = new SQLiteCommand(mDBCon);
                cmd.CommandText = sql;
                cmd.ExecuteNonQuery();
                mDBCon.Close();
            }
        }

        public enum DbConnMode
        {
            Open,
            Closed
        }

        private SQLiteConnection GetConnection(DbConnMode mode = DbConnMode.Open)
        {
            SQLiteConnection mDBcon = new SQLiteConnection();
            mDBcon.ConnectionString = this.ConnectionString;
            if (mode == DbConnMode.Open)
                mDBcon.Open();
            return mDBcon;
        }

        public void AddLayer(string layerName, int layerid, int parentLayerId, byte[] colour, byte[] vbo, byte[] bbArray)
        {
            if (_transactedLayer == null)
            {
                Flush();
                string str = "INSERT INTO Layers (SceneName, LayerName, LayerId, ParentLayerId, Meshes, XbimTexture, BoundingBox) " +
                         "VALUES (@SceneName, @LayerName, @LayerId, @ParentLayerId, @Meshes, @XbimTexture, @BoundingBox) ";

                _transactedLayer = new transactedCommand(this, str);
                _transactedLayer.Command.Parameters.Add("@SceneName", DbType.String).Value = "MainScene";
                _transactedLayer.Command.Parameters.Add("@LayerName", DbType.String);
                _transactedLayer.Command.Parameters.Add("@LayerId", DbType.Int32);
                _transactedLayer.Command.Parameters.Add("@ParentLayerId", DbType.Int32);
                _transactedLayer.Command.Parameters.Add("@Meshes", DbType.Binary);
                _transactedLayer.Command.Parameters.Add("@XbimTexture", DbType.Binary);
                _transactedLayer.Command.Parameters.Add("@BoundingBox", DbType.Binary);
            }

            _transactedLayer.Command.Parameters["@LayerName"].Value = layerName;
            _transactedLayer.Command.Parameters["@LayerId"].Value = layerid;
            _transactedLayer.Command.Parameters["@ParentLayerId"].Value = parentLayerId;
            _transactedLayer.Command.Parameters["@Meshes"].Value = vbo;
            _transactedLayer.Command.Parameters["@XbimTexture"].Value = colour;
            _transactedLayer.Command.Parameters["@BoundingBox"].Value = bbArray;
            _transactedLayer.Command.ExecuteNonQuery();
        }

        
        /// <summary>
        /// Adds a receord to the Meta table
        /// </summary>
        /// <param name="Type">Type of record (required)</param>
        /// <param name="EntityLabel">Allows for entitylabel lookup when appropriate</param>
        /// <param name="Identifier">Optional</param>
        /// <param name="Value">Any string persistence mechanism of choice (required).</param>
        public void AddMetaData(string Type, int EntityLabel, string Value, string Identifier = null)
        {
            if (_transactedMetaString == null)
            {
                Flush();
                string str = "INSERT INTO Meta (" +
                    "'Meta_type', 'Meta_Value', 'Meta_key', 'Meta_intkey' " +
                    ") values (" +
                    "@Meta_type, @Meta_Value, @Meta_key, @Meta_intkey  " +
                    ")";
                _transactedMetaString = new transactedCommand(this, str);
                _transactedMetaString.Command.Parameters.Add("@Meta_type", DbType.String);
                _transactedMetaString.Command.Parameters.Add("@Meta_Value", DbType.String);
                _transactedMetaString.Command.Parameters.Add("@Meta_key", DbType.String);
                _transactedMetaString.Command.Parameters.Add("@Meta_intkey", DbType.Int32);
            }

            _transactedMetaString.Command.Parameters["@Meta_type"].Value = Type;
            _transactedMetaString.Command.Parameters["@Meta_Value"].Value = Value;
            if (Identifier == null)
                _transactedMetaString.Command.Parameters["@Meta_key"].Value = DBNull.Value;
            else
                _transactedMetaString.Command.Parameters["@Meta_key"].Value = Identifier;
            _transactedMetaString.Command.Parameters["@Meta_intkey"].Value = EntityLabel;
            _transactedMetaString.Command.ExecuteNonQuery();
        }

        private transactedCommand _transactedMetaByteArray;
        private transactedCommand _transactedMetaString;
        private transactedCommand _transactedLayer;

        private class transactedCommand : IDisposable
        {
            private SQLiteConnection _cn;
            private SQLiteTransaction _trns;
            internal SQLiteCommand Command;

            public enum CmdMode
            {
                MetaBinary,
                MetaString
            }

            public transactedCommand(XbimSqliteDB db, string sql)
            {
                _cn = db.GetConnection();
                _trns = _cn.BeginTransaction();
                Command = _cn.CreateCommand();
                Command.CommandText = sql;
            }

            public void Dispose()
            {
                if (_trns != null)
                {
                    _trns.Commit();
                    _trns.Dispose();
                    _trns = null;
                }
                if (Command != null)
                {
                    Command.Dispose();
                    Command = null;
                }
                if (_cn != null)
                {
                    _cn.Dispose();
                    _cn = null;
                }
            }
        }

        /// <summary>
        /// Adds a receord to the Meta table
        /// </summary>
        /// <param name="Type">Type of record (required)</param>
        /// <param name="EntityLabel">Allows for entitylabel lookup when appropriate</param>
        /// <param name="Identifier">Optional</param>
        /// <param name="Value">Any string persistence mechanism of choice (required).</param>
        public void AddMetaData(string Type, int EntityLabel, byte[] Value, string Identifier = null)
        {
            if (_transactedMetaByteArray == null)
            {
                Flush();
                string str = "INSERT INTO Meta (" +
                    "'Meta_type', 'Meta_Value', 'Meta_key', 'Meta_intkey' " +
                    ") values (" +
                    "@Meta_type, @Meta_Value, @Meta_key, @Meta_intkey  " +
                    ")";
                _transactedMetaByteArray = new transactedCommand(this, str);
                _transactedMetaByteArray.Command.Parameters.Add("@Meta_type", DbType.String);
                _transactedMetaByteArray.Command.Parameters.Add("@Meta_Value", DbType.Binary);
                _transactedMetaByteArray.Command.Parameters.Add("@Meta_key", DbType.String);
                _transactedMetaByteArray.Command.Parameters.Add("@Meta_intkey", DbType.Int32);
            }

            _transactedMetaByteArray.Command.Parameters["@Meta_type"].Value = Type;
            _transactedMetaByteArray.Command.Parameters["@Meta_Value"].Value = Value;
            if (Identifier == null)
                _transactedMetaByteArray.Command.Parameters["@Meta_key"].Value = DBNull.Value;
            else
                _transactedMetaByteArray.Command.Parameters["@Meta_key"].Value = Identifier;
            _transactedMetaByteArray.Command.Parameters["@Meta_intkey"].Value = EntityLabel;
            _transactedMetaByteArray.Command.ExecuteNonQuery();
        }

        public void Dispose()
        {
            Flush();
        }

        public void Flush()
        {
            if (_transactedMetaByteArray != null)
            {
                _transactedMetaByteArray.Dispose();
                _transactedMetaByteArray = null;
            }
            if (_transactedMetaString != null)
            {
                _transactedMetaString.Dispose();
                _transactedMetaString = null;
            }
            if (_transactedLayer != null)
            {
                _transactedLayer.Dispose();
                _transactedLayer = null;
            }
        }

        public void AddScene(int p1, string p2, byte[] boundingBoxFull)
        {
            Flush();
            string str =
                        "INSERT INTO Scenes (SceneId, SceneName, BoundingBox) " +
                        "VALUES (@SceneId, @SceneName, @BoundingBox) ";
            using (SQLiteConnection connection = this.GetConnection())
            {
                using (SQLiteTransaction SQLiteTrans = connection.BeginTransaction())
                {
                    using (SQLiteCommand cmd = connection.CreateCommand())
                    {
                        cmd.CommandText = str;
                        cmd.Parameters.Add("@SceneId", DbType.Int32).Value = 1;
                        cmd.Parameters.Add("@SceneName", DbType.String).Value = "MainScene";
                        cmd.Parameters.Add("@BoundingBox", DbType.Binary).Value = boundingBoxFull;
                        cmd.ExecuteNonQuery();
                    }
                    SQLiteTrans.Commit();
                }
            }
        }
    }
}
