abstract class Character
{
    private readonly string characterType;

    protected Character(string characterType)
    {
        this.characterType = characterType;
    }

    public abstract int DamagePoints(Character target);

    public virtual bool Vulnerable() => false;

    public override string ToString() => $"Character is a {characterType}";
}

class Warrior : Character
{
    private const int RegularDamage = 6;
    private const int VulnerableTargetDamage = 10;

    public Warrior() : base(nameof(Warrior))
    {
    }

    public override int DamagePoints(Character target) =>
        target.Vulnerable() ? VulnerableTargetDamage : RegularDamage;
}

class Wizard : Character
{
    private const int PreparedDamage = 12;
    private const int UnpreparedDamage = 3;

    private bool spellPrepared;

    public Wizard() : base(nameof(Wizard))
    {
    }

    public override int DamagePoints(Character target) =>
        spellPrepared ? PreparedDamage : UnpreparedDamage;

    public override bool Vulnerable() => !spellPrepared;

    public void PrepareSpell()
    {
        spellPrepared = true;
    }
}
