using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Igra : MonoBehaviour
{

    public int znak;
    public int broj;

    public Karta karta;

    public List<Karta> spil = new List<Karta>();

    public Kolona[] kolone;

    public Ruka ruka;

    public GameObject bravo;
    public GameObject zvuk;
    public GameObject mute;
    public GameObject restart;

    private bool mjuza = true;

    public void mesanje<Karta>(List<Karta> list)
    {
        int n = list.Count;
        while (n > 1)
        {
            n--;
            int k = (int)Random.Range(0f, n + 1f);
            Karta pomocna = list[k];
            list[k] = list[n];
            list[n] = pomocna;
        }
    }


    void Start()
    {
        //zavrsna poruka
        bravo.GetComponent<SpriteRenderer>().enabled = false;

        //pravljenje spila (52 karte)
        for (znak = 1; znak <= 4; znak++)
        {
            for (broj = 1; broj <= 13; broj++)
            {
                Karta k = Instantiate(karta);
                spil.Add(k);
            }
        }

        //mesanje spila
        mesanje(spil);

        //punjenje kolona iz spila
        for (int i = 0; i < kolone.Length; i++)
        {
            for (int j = 0; j <= i; j++)
            {
                Karta k = spil[j];
                kolone[i].karte.Add(k);
            }
            spil.RemoveRange(0, i + 1);
        }

        //punjenje Ruke i praznjenje spila       
        ruka.karteZatvorene.AddRange(spil);
        spil.Clear();
        //kako bi uvek u zatvorenim kartama Ruke pisalo "sledeca"
        Karta visak = Instantiate(karta);
        ruka.karteZatvorene.Add(visak);
        for (int i = 0; i < ruka.karteZatvorene.Count; i++)
        {

            if (i == ruka.karteZatvorene.Count - 1)
                ruka.karteZatvorene[i].ispis.sprite = ruka.karteZatvorene[i].slikaPoledjine;
            else
                ruka.karteZatvorene[i].ispis.sprite = null;
        }

        //pozicioniranje kolona
        float x = -4f;
        float y = 4.5f;
        float z = 0f;

        for (int i = 0; i < kolone.Length; i++)
        {
            kolone[i].transform.position = new Vector3((i + 1) * 1f + x, -0.5f + y, 0f);
        }

        //pozicioniranje karata u koloni
        for (int i = 0; i < kolone.Length; i++)
        {
            for (int j = 0; j < kolone[i].karte.Count; j++)
            {
                kolone[i].karte[j].transform.position =
                    new Vector3((i + 1) * 1f + x, (j + 1) * -0.4f + y, z - (j + 1) * 0.01f);
            }
        }

        //pozicioniranje ruke

        foreach (Karta k in ruka.GetComponent<Ruka>().karteZatvorene)
        {
            k.transform.position = new Vector3(5f, 1.7f, 0f);
        }


    }

    public void proveraKrajaIgre()
    {
        bool izlaz = false;
        restart.GetComponent<SpriteRenderer>().enabled = false;
        mute.GetComponent<SpriteRenderer>().enabled = false;
        for (int i = 0; i < kolone.Length; i++)
        {
            for (int j = 0; j < kolone[i].karte.Count; j++)
            {
                if (!kolone[i].karte[j].okrenuta)
                {
                    izlaz = true;
                    break;
                }
            }
            if (izlaz)
            {
                break;
            }
        }
        if (!izlaz)
        {
            bravo.GetComponent<SpriteRenderer>().enabled = true;
            restart.GetComponent<SpriteRenderer>().enabled = true;
        }
    }

    void Update()
    {

        if (Input.GetKeyDown(KeyCode.R))
        {
            UnityEngine.SceneManagement.SceneManager.LoadScene("ScenaZaPasijans");
        }
        if (Input.GetKeyDown(KeyCode.M))
        {
            if (mjuza)
            {
                zvuk.GetComponent<AudioSource>().volume = 0f;
                mjuza = false;
            }

            else
            {
                zvuk.GetComponent<AudioSource>().volume = 0.5f;
                mjuza = true;
            }
        }

        // okretanje poslednje karte u koloni ukoliko je neokrenuta
        for (int i = 0; i < kolone.Length; i++)
        {
            for (int j = 0; j < kolone[i].karte.Count; j++)
            {
                if (j == kolone[i].karte.Count - 1 && !kolone[i].karte[j].okrenuta)
                {
                    kolone[i].karte[j].okrenuta = true;
                }
            }
        }
    }




}
