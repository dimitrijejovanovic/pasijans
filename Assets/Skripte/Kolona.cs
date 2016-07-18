using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Kolona : MonoBehaviour
{

    public List<Karta> karte = new List<Karta>();
    public float x;
    public float y;
    public Vector3 pocetak;
    Collider sc;
    public GameObject igra;
    public GameObject ruka;


    // Use this for initialization
    void Awake()
    {
        ruka = GameObject.FindGameObjectWithTag("Ruka");
        igra = GameObject.FindGameObjectWithTag("Igra");
        sc = GetComponent<SphereCollider>();
        pocetak = new Vector3(x, y, 0f);
        transform.position = pocetak;
    }

    // Update is called once per frame
    void Update()
    {
        //omogucavanje dodavanja karata samo na poslednju kartu kolone; 
        for (int i = 0; i < karte.Count; i++)
        {
            if (i != karte.Count - 1)
            {
                karte[i].sc.isTrigger = false;
                karte[i].sc.enabled = false;
            }
            else
                karte[i].sc.enabled = true;
            karte[i].sc.isTrigger = true;
        }
    }

    //dodavanje na pocetak kolone
    void OnTriggerEnter(Collider other)
    {
        if (karte.Count == 0 &&
            other.GetComponent<Karta>().okrenuta &&
            other.GetComponent<Karta>().broj == 13
            )
        {
            //izbacivanje iz prvobitne kolone
            bool prvaPetlja = false;
            bool drugaPetlja = false;
            bool nijePoslednja = false;

            // provera da li je u Ruci i izbacivanje iz Ruke
            foreach (Karta k in ruka.GetComponent<Ruka>().karteVidljive)
            {
                if (k == other.GetComponent<Karta>())
                {
                    ruka.GetComponent<Ruka>().karteVidljive.Remove(other.GetComponent<Karta>());
                    prvaPetlja = true;
                }
                if (prvaPetlja) break;
            }

            prvaPetlja = false;
            drugaPetlja = false;

            int indeks = 100;
            List<Karta> pomocnaLista = new List<Karta>();

            //Provera da li je karta u kolonoma (preskace se ukoliko je u ruci)
            if (!prvaPetlja)
            {
                foreach (Kolona lk in igra.GetComponent<Igra>().kolone)
                {
                    foreach (Karta k in lk.karte)
                    {
                        if (other.GetComponent<Karta>() == k)
                        {
                            indeks = lk.karte.IndexOf(other.GetComponent<Karta>());
                        }
                        if (indeks != 100 && indeks != lk.karte.Count - 1)
                        {
                            pomocnaLista = lk.karte.GetRange(indeks, lk.karte.Count - indeks);
                            lk.karte.RemoveRange(indeks, lk.karte.Count - indeks);
                            prvaPetlja = true;
                            drugaPetlja = true;
                            nijePoslednja = true;
                        }

                        if (indeks != 100)
                        {
                            lk.karte.Remove(other.GetComponent<Karta>());
                            prvaPetlja = true;
                            drugaPetlja = true;
                        }

                        if (drugaPetlja) break;
                    }
                    if (prvaPetlja) break;
                }
            }
            prvaPetlja = false;
            drugaPetlja = false;

            //ubacivanje u buducu kolonu
            if (!nijePoslednja)
            {
                karte.Add(other.GetComponent<Karta>());
                other.GetComponent<Karta>().uRuciOtvorena = false;
                other.GetComponent<Karta>().uRuciZatvorena = false;
                prvaPetlja = true;
                drugaPetlja = true;
            }
            if (nijePoslednja)
            {
                karte.AddRange(pomocnaLista);
                foreach (Karta item in pomocnaLista)
                {
                    item.uRuciOtvorena = false;
                    item.uRuciZatvorena = false;
                    prvaPetlja = true;
                    drugaPetlja = true;
                }

            }

        }
    }
}