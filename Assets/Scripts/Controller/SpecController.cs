using UnityEngine;
using System;
using System.Text;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Xml;
using System.Xml.Serialization;
using System.Reflection;

public class SpecController{
	public const int FishType = 1;
	public const int ItemType = 2;
	public const int BulletType = 3;
	public const int TreasureType = 4;
	public const int CannonType = 5;
	public const int BulletItemType = 6;
	public const int TaskType = 7;
	static Dictionary<string,Dictionary<string,ItemSpec>> items = new Dictionary<string, Dictionary<string,ItemSpec>> ();

	public static void initGameXML(){
		string XMLUrl = "game";
		XmlDocument xmlDoc = new XmlDocument();  
		Debug.Log("加载game xml文档");
		xmlDoc.LoadXml (Resources.Load (XMLUrl).ToString());

		XmlNode languageNodes = xmlDoc.SelectSingleNode("database");  
		foreach (XmlElement node in languageNodes)  
		{
			Dictionary<string,ItemSpec> groups = new  Dictionary<string, ItemSpec> ();

			string groupName=node.GetAttribute ("group_id");
			System.Type type = getSpecType(groupName);
			ConstructorInfo ct = type.GetConstructor(System.Type.EmptyTypes);


			FieldInfo[] fields = type.GetFields();

			foreach (XmlElement element in node)  
			{
				ItemSpec target = ct.Invoke(null) as ItemSpec;
				string fieldName = string.Empty;
				string id = string.Empty;
				foreach(FieldInfo f in fields)
				{
					fieldName = f.Name;
					if(element.HasAttribute(fieldName))
					{
						if(fieldName == "item_id"){
							id =  element.GetAttribute (fieldName);
						}
						f.SetValue(target, Convert.ChangeType(element.GetAttribute(fieldName), f.FieldType));
					}
				}
				groups.Add (id,target);
			}
			items.Add (groupName, groups);
		}
	
	}

	private static System.Type getSpecType(string name){
		System.Type type;
		switch (name) {
		case "Fish":
			type = typeof(FishItemSpec);
			break;
		case "Bullet":
			type = typeof(BulletItemSpec);
			break;
		case "Cannon":
			type = typeof(CannonItemSpec);
			break;
		case "Task":
			type = typeof(TaskItemSpec);
			break;
		default:
			type = typeof(ItemSpec);
			break;
		}
		return type;
	}

	public static ItemSpec getItemById(string id){
		string group  = "Item";
		int t =  int.Parse(id) / 10000;
		switch (t) {
		case 1:
			group = "Fish";
			break;
		case 2:
			group = "Item";
			break;
		case 3:
			group = "Bullet";
			break;
		case 4:
			group = "Treasure";
			break;
		case 5:
			group = "Cannon";
			break;
		case 6:
			group = "BulletItem";
			break;
		case 7:
			group = "Task";
			break;
		}
		if (!items.ContainsKey (group)) {
			Debug.Log ("no Grop: " + group);
			return null;
		} else if (!items [group].ContainsKey (id)) {
			Debug.Log ("no item in grop: " + group + "  id " + id);
			return null;
		} 
		return items[group][id];

	}
	public static Dictionary<string,ItemSpec> getGroup(string group){
		return items[group];
	}

}
