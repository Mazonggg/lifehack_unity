﻿using System;
using Lifehack.Model.Konstanten;

namespace Lifehack.Model {

    public abstract class DatenbankEintrag : IDatenbankEintrag {

        string id;
        public string Id {
            get { return this.id; }
            set { this.id = value; }
        }

        protected DatenbankEintrag() { }

        abstract public TabellenName Tabelle();
    }
}

