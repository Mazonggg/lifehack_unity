﻿
using Lifehack.Model.Einrichtung;
using SimpleJSON;
using System;
using Lifehack.Model.Prozess;
using System.Collections.Generic;

namespace Lifehack.Model.Fabrik.Prozess {

    public class AufgabeFabrik : DatenbankEintragFabrik<Aufgabe> {

        static AufgabeFabrik _instance;

        AufgabeFabrik() { }

        public static AufgabeFabrik Instance {
            get {
                if (AufgabeFabrik._instance == null) {
                    AufgabeFabrik._instance = new AufgabeFabrik();
                }
                return AufgabeFabrik._instance;
            }
        }

        public override Aufgabe ErzeugeLeeresEintragObjekt() {
            return new Aufgabe();
        }

        protected override Aufgabe SetAttribute(Aufgabe datenbankEintrag, JSONObject json) {
            datenbankEintrag.Id = json["aufgabe_id"].Value;
            datenbankEintrag.Bezeichnung = json["bezeichnung"].Value;
            datenbankEintrag.Gesetzesgrundlage = json["gesetzesgrundlage"].Value;
            var teilaufgaben = new List<Teilaufgabe>();
            foreach (JSONObject teilaufgabeDaten in json["teilaufgabe"]) {
                datenbankEintrag.Gesetzesgrundlage += " # ";
                Teilaufgabe teilaufgabe = (Teilaufgabe)TeilaufgabeFabrik.Instance.ErzeugeDantebankEintrag(teilaufgabeDaten.AsObject);
                teilaufgabe.Aufgabe = datenbankEintrag;
                teilaufgaben.Add(teilaufgabe);
            }
            datenbankEintrag.Teilaufgaben = teilaufgaben.ToArray();
            return datenbankEintrag;
        }
    }
}

