using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Ruka : MonoBehaviour
{

    public List<Karta> karteZatvorene = new List<Karta>();
    public List<Karta> karteVidljive = new List<Karta>();

    Vector3 prvobitnaPozicija;

    public GameObject igra;

    public Sprite sledeca;

    // Use this for initialization
    void Awake()
    {
        igra = GameObject.FindGameObjectWithTag("Igra");
        prvobitnaPozicija = new Vector3(5f, 1.7f, 0f);
        transform.position = prvobitnaPozicija;
        foreach (Karta k in karteZatvorene)
        {
            k.transform.position = transform.position;
        }
    }

    public void proveraRuke()
    {
        for (int i = 0; i < karteZatvorene.Count; i++)
        {
            karteZatvorene[i].transform.position = transform.position;
            karteZatvorene[i].okrenuta = false;
            karteZatvorene[i].uRuciOtvorena = false;
            karteZatvorene[i].uRuciZatvorena = true;
            if (i == karteZatvorene.Count - 1)
                karteZatvorene[i].ispis.sprite = karteZatvorene[i].slikaPoledjine;
            else
                karteZatvorene[i].ispis.sprite = null;
        }
        //da se poslednjoj vidi ispis
        for (int i = 0; i < karteVidljive.Count; i++)
        {
            if (i == karteVidljive.Count - 1)
            {
                karteVidljive[i].transform.position = prvobitnaPozicija + new Vector3(1f, 0f, -0.2f);
                karteVidljive[i].okrenuta = true;
                karteVidljive[i].ispis.sprite = karteVidljive[i].ispisivanje();
            }

            else
            {
                karteVidljive[i].okrenuta = false;
                karteVidljive[i].transform.position = prvobitnaPozicija + new Vector3(1f, 0f, 1-i/50f);

            }
        }
    }



    void Update()
    {
        for (int i = 0; i < karteVidljive.Count; i++)
        {
            karteVidljive[i].uRuciZatvorena = false;

            //svaka treba da dobije bool uRuciOtvorena da bude true
            if (karteVidljive[i].uRuciOtvorena == false)
            {
                karteVidljive[i].uRuciOtvorena = true;
            }
            if (i == karteVidljive.Count - 1)
            {
                karteVidljive[i].okrenuta = true;
            }


        }

        for (int i = 0; i < karteZatvorene.Count; i++)
        {
            //svaka zatvorena u ruci treba da dobije bool uRuciZatvorena da je true
            if (karteZatvorene[i].uRuciZatvorena == false)
                karteZatvorene[i].uRuciZatvorena = true;


        }



    }

}