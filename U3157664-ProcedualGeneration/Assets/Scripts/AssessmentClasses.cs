using UnityEngine;
using System;
using System.Collections.Generic;
using System.Linq;

public class AssessmentClasses : MonoBehaviour
{
    delegate void activeExtras();
    activeExtras addTheAnimals;
    // Use this for initialization

    List<Animal> animalList = new List<Animal>();

	void Start ()
    {

        addTheAnimals += AddCats;
        addTheAnimals += AddDogs;
        addTheAnimals += sortCat;
        addTheAnimals += sortDog;

        if (addTheAnimals != null) addTheAnimals();

        Predicate<string> isCat = x => x== "cat";

        foreach (Animal item in animalList)
        {
            item.Species();
            Debug.Log(item.breed + " " + item.species);
            Debug.Log(isCat(item.species));
        } 
    }
    void sortDog()
    {
        var dog = from animal in animalList
                  where animal.species == "Canine"
                  select animal.breed;
        string[] dogs = dog.ToArray();
        foreach (string item in dogs)
        {

            Debug.Log(item + " is a dog");
        }
    }
    void sortCat()
    {

        int catCount = animalList.Where(x => x.species == "cat").Count();
        Debug.Log("there are " + catCount + " cats Here");
    }
     void AddCats()
    {
        feline cat1 = new feline();
        feline cat2 = new feline();
        feline cat3 = new feline();

        cat1.Breed("Ragdoll");
        cat2.Breed("Sphynx");
        cat3.Breed("Tiger");

        animalList.Add(cat1);
        animalList.Add(cat2);
        animalList.Add(cat3);
    }
    void AddDogs()
    {
        k9 dog1 = new k9();
        k9 dog2 = new k9();
        k9 dog3 = new k9();

        dog1.Breed("Germam Shepard");
        dog2.Breed("Border Collie");
        dog3.Breed("Husky");


        animalList.Add(dog1);
        animalList.Add(dog2);
        animalList.Add(dog3);
    }


}

abstract class Animal 
{
    public string species = "unknown";
    public string breed;

    public abstract void Species();
    public abstract void Breed(string Bbreed);


}

class k9 : Animal
{
    

    public override void Species()
    {
        species = "Canine";
    }
    public override void Breed(string Bbreed)
    {
        
        breed = Bbreed;
    }

    
}
class feline : Animal
{

    public override void Species()
    {
        species = "cat";
    }
    public override void Breed(string Bbreed)
    {

        breed = Bbreed;
    }


}

