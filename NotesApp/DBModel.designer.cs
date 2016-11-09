﻿#pragma warning disable 1591
//------------------------------------------------------------------------------
// <auto-generated>
//     Этот код создан программой.
//     Исполняемая версия:4.0.30319.42000
//
//     Изменения в этом файле могут привести к неправильной работе и будут потеряны в случае
//     повторной генерации кода.
// </auto-generated>
//------------------------------------------------------------------------------

namespace NotesApp
{
	using System.Data.Linq;
	using System.Data.Linq.Mapping;
	using System.Data;
	using System.Collections.Generic;
	using System.Reflection;
	using System.Linq;
	using System.Linq.Expressions;
	using System.ComponentModel;
	using System;
	
	
	[global::System.Data.Linq.Mapping.DatabaseAttribute(Name="u0256509_db1")]
	public partial class DBModelDataContext : System.Data.Linq.DataContext
	{
		
		private static System.Data.Linq.Mapping.MappingSource mappingSource = new AttributeMappingSource();
		
    #region Определения метода расширяемости
    partial void OnCreated();
    partial void Insertnotes(notes instance);
    partial void Updatenotes(notes instance);
    partial void Deletenotes(notes instance);
    partial void Insertusers(users instance);
    partial void Updateusers(users instance);
    partial void Deleteusers(users instance);
    #endregion
		
		public DBModelDataContext() : 
				base(global::NotesApp.Properties.Settings.Default.u0256509_db1ConnectionString, mappingSource)
		{
			OnCreated();
		}
		
		public DBModelDataContext(string connection) : 
				base(connection, mappingSource)
		{
			OnCreated();
		}
		
		public DBModelDataContext(System.Data.IDbConnection connection) : 
				base(connection, mappingSource)
		{
			OnCreated();
		}
		
		public DBModelDataContext(string connection, System.Data.Linq.Mapping.MappingSource mappingSource) : 
				base(connection, mappingSource)
		{
			OnCreated();
		}
		
		public DBModelDataContext(System.Data.IDbConnection connection, System.Data.Linq.Mapping.MappingSource mappingSource) : 
				base(connection, mappingSource)
		{
			OnCreated();
		}
		
		public System.Data.Linq.Table<notes> notes
		{
			get
			{
				return this.GetTable<notes>();
			}
		}
		
		public System.Data.Linq.Table<users> users
		{
			get
			{
				return this.GetTable<users>();
			}
		}
	}
	
	[global::System.Data.Linq.Mapping.TableAttribute(Name="u0256509_user.notes")]
	public partial class notes : INotifyPropertyChanging, INotifyPropertyChanged
	{
		
		private static PropertyChangingEventArgs emptyChangingEventArgs = new PropertyChangingEventArgs(String.Empty);
		
		private int _id;
		
		private int _createdby;
		
		private string _text;
		
		private System.Nullable<System.DateTime> _lastedit;
		
		private EntityRef<users> _users;
		
    #region Определения метода расширяемости
    partial void OnLoaded();
    partial void OnValidate(System.Data.Linq.ChangeAction action);
    partial void OnCreated();
    partial void OnidChanging(int value);
    partial void OnidChanged();
    partial void OncreatedbyChanging(int value);
    partial void OncreatedbyChanged();
    partial void OntextChanging(string value);
    partial void OntextChanged();
    partial void OnlasteditChanging(System.Nullable<System.DateTime> value);
    partial void OnlasteditChanged();
    #endregion
		
		public notes()
		{
			this._users = default(EntityRef<users>);
			OnCreated();
		}
		
		[global::System.Data.Linq.Mapping.ColumnAttribute(Storage="_id", AutoSync=AutoSync.OnInsert, DbType="Int NOT NULL IDENTITY", IsPrimaryKey=true, IsDbGenerated=true)]
		public int id
		{
			get
			{
				return this._id;
			}
			set
			{
				if ((this._id != value))
				{
					this.OnidChanging(value);
					this.SendPropertyChanging();
					this._id = value;
					this.SendPropertyChanged("id");
					this.OnidChanged();
				}
			}
		}
		
		[global::System.Data.Linq.Mapping.ColumnAttribute(Storage="_createdby", DbType="Int NOT NULL")]
		public int createdby
		{
			get
			{
				return this._createdby;
			}
			set
			{
				if ((this._createdby != value))
				{
					if (this._users.HasLoadedOrAssignedValue)
					{
						throw new System.Data.Linq.ForeignKeyReferenceAlreadyHasValueException();
					}
					this.OncreatedbyChanging(value);
					this.SendPropertyChanging();
					this._createdby = value;
					this.SendPropertyChanged("createdby");
					this.OncreatedbyChanged();
				}
			}
		}
		
		[global::System.Data.Linq.Mapping.ColumnAttribute(Storage="_text", DbType="NText", UpdateCheck=UpdateCheck.Never)]
		public string text
		{
			get
			{
				return this._text;
			}
			set
			{
				if ((this._text != value))
				{
					this.OntextChanging(value);
					this.SendPropertyChanging();
					this._text = value;
					this.SendPropertyChanged("text");
					this.OntextChanged();
				}
			}
		}
		
		[global::System.Data.Linq.Mapping.ColumnAttribute(Storage="_lastedit", DbType="DateTime")]
		public System.Nullable<System.DateTime> lastedit
		{
			get
			{
				return this._lastedit;
			}
			set
			{
				if ((this._lastedit != value))
				{
					this.OnlasteditChanging(value);
					this.SendPropertyChanging();
					this._lastedit = value;
					this.SendPropertyChanged("lastedit");
					this.OnlasteditChanged();
				}
			}
		}
		
		[global::System.Data.Linq.Mapping.AssociationAttribute(Name="users_notes", Storage="_users", ThisKey="createdby", OtherKey="id", IsForeignKey=true)]
		public users users
		{
			get
			{
				return this._users.Entity;
			}
			set
			{
				users previousValue = this._users.Entity;
				if (((previousValue != value) 
							|| (this._users.HasLoadedOrAssignedValue == false)))
				{
					this.SendPropertyChanging();
					if ((previousValue != null))
					{
						this._users.Entity = null;
						previousValue.notes.Remove(this);
					}
					this._users.Entity = value;
					if ((value != null))
					{
						value.notes.Add(this);
						this._createdby = value.id;
					}
					else
					{
						this._createdby = default(int);
					}
					this.SendPropertyChanged("users");
				}
			}
		}
		
		public event PropertyChangingEventHandler PropertyChanging;
		
		public event PropertyChangedEventHandler PropertyChanged;
		
		protected virtual void SendPropertyChanging()
		{
			if ((this.PropertyChanging != null))
			{
				this.PropertyChanging(this, emptyChangingEventArgs);
			}
		}
		
		protected virtual void SendPropertyChanged(String propertyName)
		{
			if ((this.PropertyChanged != null))
			{
				this.PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
			}
		}
	}
	
	[global::System.Data.Linq.Mapping.TableAttribute(Name="u0256509_user.users")]
	public partial class users : INotifyPropertyChanging, INotifyPropertyChanged
	{
		
		private static PropertyChangingEventArgs emptyChangingEventArgs = new PropertyChangingEventArgs(String.Empty);
		
		private int _id;
		
		private string _username;
		
		private string _password;
		
		private EntitySet<notes> _notes;
		
    #region Определения метода расширяемости
    partial void OnLoaded();
    partial void OnValidate(System.Data.Linq.ChangeAction action);
    partial void OnCreated();
    partial void OnidChanging(int value);
    partial void OnidChanged();
    partial void OnusernameChanging(string value);
    partial void OnusernameChanged();
    partial void OnpasswordChanging(string value);
    partial void OnpasswordChanged();
    #endregion
		
		public users()
		{
			this._notes = new EntitySet<notes>(new Action<notes>(this.attach_notes), new Action<notes>(this.detach_notes));
			OnCreated();
		}
		
		[global::System.Data.Linq.Mapping.ColumnAttribute(Storage="_id", AutoSync=AutoSync.OnInsert, DbType="Int NOT NULL IDENTITY", IsPrimaryKey=true, IsDbGenerated=true)]
		public int id
		{
			get
			{
				return this._id;
			}
			set
			{
				if ((this._id != value))
				{
					this.OnidChanging(value);
					this.SendPropertyChanging();
					this._id = value;
					this.SendPropertyChanged("id");
					this.OnidChanged();
				}
			}
		}
		
		[global::System.Data.Linq.Mapping.ColumnAttribute(Storage="_username", DbType="VarChar(30)")]
		public string username
		{
			get
			{
				return this._username;
			}
			set
			{
				if ((this._username != value))
				{
					this.OnusernameChanging(value);
					this.SendPropertyChanging();
					this._username = value;
					this.SendPropertyChanged("username");
					this.OnusernameChanged();
				}
			}
		}
		
		[global::System.Data.Linq.Mapping.ColumnAttribute(Storage="_password", DbType="VarChar(30)")]
		public string password
		{
			get
			{
				return this._password;
			}
			set
			{
				if ((this._password != value))
				{
					this.OnpasswordChanging(value);
					this.SendPropertyChanging();
					this._password = value;
					this.SendPropertyChanged("password");
					this.OnpasswordChanged();
				}
			}
		}
		
		[global::System.Data.Linq.Mapping.AssociationAttribute(Name="users_notes", Storage="_notes", ThisKey="id", OtherKey="createdby")]
		public EntitySet<notes> notes
		{
			get
			{
				return this._notes;
			}
			set
			{
				this._notes.Assign(value);
			}
		}
		
		public event PropertyChangingEventHandler PropertyChanging;
		
		public event PropertyChangedEventHandler PropertyChanged;
		
		protected virtual void SendPropertyChanging()
		{
			if ((this.PropertyChanging != null))
			{
				this.PropertyChanging(this, emptyChangingEventArgs);
			}
		}
		
		protected virtual void SendPropertyChanged(String propertyName)
		{
			if ((this.PropertyChanged != null))
			{
				this.PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
			}
		}
		
		private void attach_notes(notes entity)
		{
			this.SendPropertyChanging();
			entity.users = this;
		}
		
		private void detach_notes(notes entity)
		{
			this.SendPropertyChanging();
			entity.users = null;
		}
	}
}
#pragma warning restore 1591