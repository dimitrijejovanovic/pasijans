using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Talon : MonoBehaviour
{
    public List<Karta> karte = new List<Karta>();

    Collider sc;
    public GameObject igra;
    public GameObject ruka;

    

    // Use this for initialization
    void Awake()
    {
        ruka = GameObject.FindGameObjectWithTag("Ruka");
        igra = GameObject.FindGameObjectWithTag("Igra");
        
        sc = GetComponent<SphereCollider>();
        
    }


    void OnTriggerEnter(Collider other)
    {
        if (
            karte.Count == 0 &&
            other.GetComponent<Karta>().broj == 1
            ||
            karte.Count > 0 &&
            other.GetComponent<Karta>().broj - karte.Count == 1 &&
            other.GetComponent<Karta>().znak == karte[karte.Count - 1].znak
            )
        {
            bool prvaPetlja = false;
            bool drugaPetlja = false;
            bool uslovIspunjen = false;

            // provera da li je u Ruci i izbacivanje
            foreach (Karta k in ruka.GetComponent<Ruka>().karteVidljive)
            {
                if (k == other.GetComponent<Karta>())
                {
                    uslovIspunjen = true;
                    ruka.GetComponent<Ruka>().karteVidljive.Remove(other.GetComponent<Karta>());
                    prvaPetlja = true;
                }
                if (prvaPetlja) break;
            }

            prvaPetlja = false;

            //provera da li je prinesena karta u koloni i poslednja u koloni
            if (!uslovIspunjen) {
                foreach (Kolona lk in igra.GetComponent<Igra>().kolone)
                {
                    foreach (Karta k in lk.karte)
                    {
                        if (other.GetComponent<Karta>() == k && lk.karte.Count - 1 == lk.karte.IndexOf(k))
                        {
                            uslovIspunjen = true;
                            lk.karte.Remove(other.GetComponent<Karta>());
                            prvaPetlja = true;
                            drugaPetlja = true;
                        }
                        if (drugaPetlja) break;
                    }
                    if (prvaPetlja) break;
                }
            }

            

            //ubacivanje u kolonu 
            if (uslovIspunjen)
            {
                karte.Add(other.GetComponent<Karta>());
                other.GetComponent<Karta>().okrenuta = false;
                other.GetComponent<Karta>().uRuciOtvorena = false;
                other.GetComponent<Karta>().uRuciZatvorena = false;
                other.GetComponent<Karta>().uTalonu = true;

                //provera za izbacivanje iz kolona 

                foreach (Kolona lk in igra.GetComponent<Igra>().kolone)
                {
                    foreach (Karta k in lk.karte)
                    {
                        if (other.GetComponent<Karta>() == k)
                        {
                                lk.karte.Remove(other.GetComponent<Karta>());
                        }
                    }
                }
            }
        }
    }

    void Update()
    {
        //brisanje textMash.texta prethodnih karata
        for (int i = 0; i < karte.Count; i++)
        {
            karte[i].GetComponent<Karta>().transform.position = gameObject.transform.position;
            if (i != karte.Count - 1)
                karte[i].ispis.sprite = null;
            else
                karte[i].ispis.sprite = karte[i].ispisivanje();
        }
    }
}