﻿
using System.Collections.Generic;
using SimpleJSON;
using System;
using Lifehack.Model;
using Lifehack.Spiel.Gui.Stadtplan.Kachel;
using Lifehack.Austauschformat;
using UnityEngine;
using Lifehack.Spiel.Gui.Menue;
using Lifehack.Model.Stadtplan;
using Lifehack.Spiel.Gui.Form;
using Lifehack.Spiel.Gui.Stadtplan.Model.Stadtplan;
using Lifehack.Model.Konstanten;

namespace Lifehack.Spiel.Gui.Stadtplan {

    public class StadtplanModulAdapter : ModulAdapter<IKartenelement, IKartenelement> {

        public GameObject kartenelementKachelPrefab;
        public Sprite[] kartenelementSprites;

        static StadtplanModulAdapter _instance;
        public static StadtplanModulAdapter Instance {
            get { return StadtplanModulAdapter._instance; }
        }

        void Start() {
            StadtplanModulAdapter._instance = this;

            JSONNode json = AustauschAbrufer.Instance.Json;

            this.SetKonfiguration(json[AustauschKonstanten.KONFIGURATION]);
            this.SammleAbmessungen(json[AustauschKonstanten.KARTE]);
            this.PlatziereKacheln(ModelHandler.Instance.Kartenelemente);
        }

        Dictionary<string, Abmessung> abmessungen = new Dictionary<string, Abmessung>();
        int kachelGroesse;
        public int KachelGroesse {
            get { return this.kachelGroesse; }
        }

        public Abmessung GetAbmessung(string identifier) {
            return this.abmessungen[identifier];
        }

        public override void LeereInhalt() {
            return;
        }

        void SetKonfiguration(JSONNode jsonKonfiguration) {
            Int32.TryParse(jsonKonfiguration[AustauschKonstanten.KACHEL_GROESSE].Value, out this.kachelGroesse);
        }

        void SammleAbmessungen(JSONNode jsonKarte) {
            foreach (string identifier in jsonKarte.Keys) {
                var abmessung = SimpleAbmessungFabrik.ErzeugeAbmessung(jsonKarte, identifier);
                this.abmessungen.Add(identifier, abmessung);
            }
        }

        void PlatziereKacheln(List<IKartenelement> kartenelemente) {
            var eintraege = this.ErzeugeEintragAdapters(kartenelemente);
            foreach (GameObject eintrag in eintraege) {
                eintrag.transform.SetParent(this.transform);
            }
        }

        public override void GetInhalt(List<IKartenelement> eintraege) {
            if (MenueModulAdapter.Instance.gameObject.activeInHierarchy) {
                MenueModulAdapter.Instance.SchliesseModul();
                IDatenbankEintrag datenbankEintrag = eintraege[0];
                List<IDatenbankEintrag> datenbankEintraege = new List<IDatenbankEintrag>();
                foreach (var eintrag in eintraege) {
                    datenbankEintraege.Add(eintrag);
                }
                FormModulAdapter.Instance.GetInhalt(datenbankEintraege);
            }
        }

        public override string GetPopupTitel() {
            return "StadtplanModul";
        }

        protected override GameObject ErzeugeEintragAdapter(IKartenelement datenbankEintrag) {
            int kachelId = 0;
            GameObject neuesGameObject = new GameObject();
            neuesGameObject.name = EnumHandler.AlsString(datenbankEintrag.Tabelle());
            foreach (Rect feld in StadtplanModulAdapter.Instance.GetAbmessung(datenbankEintrag.Id).Felder) {
                var kachel = Instantiate(this.kartenelementKachelPrefab);
                if (typeof(Umwelt).IsAssignableFrom(datenbankEintrag.GetType())) {
                    kachel.AddComponent<UmweltKachelAdapter>();
                    kachel.GetComponent<UmweltKachelAdapter>().Kartenelement = (Umwelt)datenbankEintrag;
                } else {
                    kachel.AddComponent<GebaeudeKachelAdapter>();
                    kachel.GetComponent<GebaeudeKachelAdapter>().Kartenelement = (Gebaeude)datenbankEintrag;
                }
                kachel.name = datenbankEintrag.KartenelementArt.ToString() + "-" + datenbankEintrag.Id + "_" + kachelId++;
                var sprite = this.GetSprite(datenbankEintrag.KartenelementAussehen);
                kachel.GetComponent<SpriteRenderer>().sprite = sprite;
                kachel.transform.position = new Vector3(feld.x + (feld.width / 2), feld.y - (feld.height / 2), -1);
                kachel.transform.localScale = this.GetObjektScale(sprite, feld.size) * 4;
                kachel.transform.SetParent(neuesGameObject.transform);
                kachel.SetActive(true);
            }
            return neuesGameObject;
        }

        Sprite GetSprite(string kartenelementAussehen) {
            foreach (Sprite sprite in this.kartenelementSprites) {
                if (sprite.name.Equals(kartenelementAussehen.Split('.')[0])) {
                    return sprite;
                }
            }
            return null;
        }

        Vector2 GetObjektScale(Sprite sprite, Vector2 feld) {
            float breite = feld.x / (sprite.rect.width / StadtplanModulAdapter.Instance.KachelGroesse);
            float hoehe = feld.y / (sprite.rect.height / StadtplanModulAdapter.Instance.KachelGroesse);
            return new Vector2(breite, hoehe);
        }
    }
}

