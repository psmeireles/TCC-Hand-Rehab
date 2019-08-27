using UnityEngine;

public enum Element {
    ICE,
    FIRE,
    LIGHTNING,
    EARTH,
    NORMAL,
    NONE
}

public class CharType {
    public Element element;
    public Color color;

    public CharType(Element element) {
        this.element = element;
        switch (element) {
            case Element.ICE:
                color = Color.blue;
                break;
            case Element.FIRE:
                color = Color.red;
                break;
            case Element.LIGHTNING:
                color = Color.yellow;
                break;
            case Element.EARTH:
                color = Color.green;
                break;
            case Element.NORMAL:
                color = Color.white;
                break;
        }
    }

    public Element getWeakness() {
        switch (element) {
            case Element.ICE:
                return Element.LIGHTNING;
            case Element.FIRE:
                return Element.ICE;
            case Element.LIGHTNING:
                return Element.EARTH;
            case Element.EARTH:
                return Element.FIRE;
        }
        return Element.NONE;
    }
}