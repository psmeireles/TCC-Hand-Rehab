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

    public CharType(Element element) {
        this.element = element;
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