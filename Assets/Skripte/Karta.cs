using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Karta : MonoBehaviour
{

    private Vector3 pozicijaNaEkranu;
    private Vector3 ofset;

    public int broj;
    public int znak;

    public Collider bc;
    public Collider sc;

    public bool okrenuta;
    public bool uTalonu;
    public bool uRuciOtvorena;
    public bool uRuciZatvorena;

    public Sprite[] slikeKarata;
    public Sprite slikaPoledjine;
    public Sprite slikaKarte;

    public SpriteRenderer ispis;

    public GameObject igra;
    public GameObject ruka;

    void Awake()
    {
        igra = GameObject.FindGameObjectWithTag("Igra");
        ruka = GameObject.FindGameObjectWithTag("Ruka");
        ispis = GetComponent<SpriteRenderer>();

        broj = igra.GetComponent<Igra>().broj;
        znak = igra.GetComponent<Igra>().znak;

        bc = GetComponent<BoxCollider>();
        sc = GetComponent<SphereCollider>();
    }


    void Update()
    {
        if (!uTalonu && !uRuciOtvorena && !uRuciZatvorena)
            ispis.sprite = ispisivanje();
    }

    //dodela slike (spritea) u zavisnosti od vrednosti broja i znaka i toga da li je okrenuta
    public Sprite ispisivanje()
    {
        if (okrenuta || uTalonu)
        {
            int sabirakPrvi = 0;
            int sabirakDrugi = 0;

            switch (znak)
            {
                case 1:
                    sabirakPrvi = 39;
                    break;
                case 2:
                    sabirakPrvi = 13;
                    break;
                case 3:
                    sabirakPrvi = 0;
                    break;
                case 4:
                    sabirakPrvi = 26; ;
                    break;
            }

            switch (broj)
            {
                case 1:
                    sabirakDrugi = 12;
                    break;
                default:
                    sabirakDrugi = broj - 2;
                    break;
            }

            return slikeKarata[sabirakPrvi + sabirakDrugi];
        }
        else
            return slikaPoledjine;
    }

    //metode za pomeranje karata prevlacenjem misa
    void OnMouseDown()
    {
        if (okrenuta)
        {
            pozicijaNaEkranu = Camera.main.WorldToScreenPoint(gameObject.transform.position);
            ofset = gameObject.transform.position - Camera.main.ScreenToWorldPoint
                (new Vector3(Input.mousePosition.x, Input.mousePosition.y, pozicijaNaEkranu.z + 0.2f));
        }
    }
    
    void OnMouseDrag()
    {
        if (okrenuta)
        {
            Vector3 trenutnaPozicijaNaEkranu = new Vector3(Input.mousePosition.x, Input.mousePosition.y, pozicijaNaEkranu.z);
            Vector3 trenutnaPozicija = Camera.main.ScreenToWorldPoint(trenutnaPozicijaNaEkranu) + ofset;
            transform.position = trenutnaPozicija;
        }
    }

    // metoda za okretanje karata u Ruci
    public void OnMouseOver()
    {
        if (Input.GetMouseButtonDown(0) && uRuciZatvorena && !okrenuta)
        {
            if (ruka.GetComponent<Ruka>().karteZatvorene.Count != 1)
            {
                Karta k = ruka.GetComponent<Ruka>().karteZatvorene[0];
                ruka.GetComponent<Ruka>().karteZatvorene.Remove(k);
                ruka.GetComponent<Ruka>().karteVidljive.Add(k);
            }
            else
            {
                ruka.GetComponent<Ruka>().karteZatvorene.InsertRange(0, ruka.GetComponent<Ruka>().karteVidljive);                
                ruka.GetComponent<Ruka>().karteVidljive.Clear();
            }
            ruka.GetComponent<Ruka>().proveraRuke();
        }
    }

    //smestanje karata na njihovo mesto se desava tek nakon otpustanja klika misa (inace bi uvek bile fiksirane) 
    void OnMouseUp()
    {
        if (okrenuta)
        {
            provera();
            ruka.GetComponent<Ruka>().proveraRuke();
            igra.GetComponent<Igra>().proveraKrajaIgre();
        }
    }

    void provera()
    {
        float x = -4f;
        float y = 4.5f;
        float z = 0f;
        for (int i = 0; i < igra.GetComponent<Igra>().kolone.Length; i++)
        {
            for (int j = 0; j < igra.GetComponent<Igra>().kolone[i].karte.Count; j++)
            {
                igra.GetComponent<Igra>().kolone[i].karte[j].transform.position =
                    new Vector3((i + 1) * 1f + x, (j + 1) * -0.4f + y, z - (j + 1) * 0.01f);
                    
            }
        }

    }

    //dodavanje u sredinu ili na kraj kolone
    void OnTriggerEnter(Collider other)
    {
        if (
            okrenuta &&
            !uRuciOtvorena &&
            other.GetComponent<Karta>().okrenuta &&
            other.GetComponent<Karta>().broj == broj - 1 &&
            other.GetComponent<Karta>().znak % 2 != znak % 2
            )
        {
            //izbacivanje iz prvobitne kolone
            bool prvaPetlja = false;
            bool drugaPetlja = false;
            bool nijePoslednja = false;
            bool kartaZaKacenjePoslednja = false;


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

            //provera da li je karta na koju se kaci karta ili karte poslednja u svojoj koloni
            foreach (Kolona lk in igra.GetComponent<Igra>().kolone)
            {
                foreach (Karta k in lk.karte)
                {
                    if(k == this && lk.karte.IndexOf(this) == lk.karte.Count - 1)
                    {
                        kartaZaKacenjePoslednja = true;
                    }
                }
            }

            int indeks = 100;
            List<Karta> pomocnaLista = new List<Karta>();

            //Provera da li je karta u kolonoma (preskace se ukoliko je u ruci)
            if (kartaZaKacenjePoslednja)
            {
                foreach (Kolona lk in igra.GetComponent<Igra>().kolone)
                {
                    
                    foreach (Karta k in lk.karte)
                    {
                        //provera da li je prinesena karta poslednja kako bi se prenele i karte ispod nje
                        if (other.GetComponent<Karta>() == k)
                        {
                            indeks = lk.karte.IndexOf(other.GetComponent<Karta>());                           
                        }
                        if(indeks != 100 && indeks != lk.karte.Count-1)
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
            if (kartaZaKacenjePoslednja)
            {
                foreach (Kolona lk in igra.GetComponent<Igra>().kolone)
                {
                    foreach (Karta k in lk.karte)
                    {
                        if (this == k)
                        {
                            if (!nijePoslednja)
                            {
                                lk.karte.Add(other.GetComponent<Karta>());
                                other.GetComponent<Karta>().uRuciOtvorena = false;
                                other.GetComponent<Karta>().uRuciZatvorena = false;
                                prvaPetlja = true;
                                drugaPetlja = true;
                            }
                            if (nijePoslednja)
                            {
                                lk.karte.AddRange(pomocnaLista);
                                foreach (Karta item in pomocnaLista)
                                {
                                    item.uRuciOtvorena = false;
                                    item.uRuciZatvorena = false;
                                    prvaPetlja = true;
                                    drugaPetlja = true;
                                }

                            }
                        }
                        if (drugaPetlja) break;
                    }
                    if (prvaPetlja) break;
                }
            }
        }
    }

}
