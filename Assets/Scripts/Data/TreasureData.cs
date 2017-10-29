using UnityEngine;
using System.Collections;

public class TreasureData
{
	public string id;
	public decimal price;
	public string priceString;
	public TreasureData(string _id,decimal _price,string _priceString){
		id = _id;
		price = _price;
		priceString = _priceString;
	}
}
