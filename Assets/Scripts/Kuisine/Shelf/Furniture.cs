using UnityEngine;
using System.Collections.Generic;

[CreateAssetMenu(fileName = "NewFurniture", menuName = "Kuisine/Furniture")]
public class Furniture : ScriptableObject
{
    public string furnitureName; 
    public int numberOfShelves;

    [System.Serializable]
    public class Shelf
    {
        public string shelfName;
        public List<Ingredient> ingredients; 
    }

    public List<Shelf> shelves = new List<Shelf>();
}
