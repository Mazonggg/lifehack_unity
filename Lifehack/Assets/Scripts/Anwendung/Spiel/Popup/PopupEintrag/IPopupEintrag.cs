﻿
namespace Lifehack.Anwendung.Spiel.Popup.PopupEintrag {

    public interface IPopupEintrag : IModulEintrag{}

    public interface IPopupEintrag<T> : IPopupEintrag {
        T Eintrag { set; }
    }
}

