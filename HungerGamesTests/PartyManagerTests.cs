using HungerGamesSimulator.Data;

namespace HungerGamesTests;

public class PartyManagerTests
{
  #region Mocks

  private class MockActor : IActor
  {
    public string Name => throw new NotImplementedException();

    public Guid ActorId { get; } = Guid.NewGuid();

    public Guid PartyId { get; set; } = Guid.Empty;

    public Coord Location => throw new NotImplementedException();

    public int Speed => throw new NotImplementedException();

    public int ArmourClass => throw new NotImplementedException();

    public int Strength => throw new NotImplementedException();

    public int Dexerity => throw new NotImplementedException();

    public int Health => throw new NotImplementedException();

    public int Charisma => throw new NotImplementedException();

    public int Wisdom => throw new NotImplementedException();

    public Weapon Weapon { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
    Coord IActor.Location { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
    int IActor.Health { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }

    public ActorAction GetNextAction( SimulationSnapshot snapshot )
    {
      throw new NotImplementedException();
    }

    public bool IsDead()
    {
      throw new NotImplementedException();
    }

    public bool IsInParty()
    {
      return PartyId != Guid.Empty;
    }

    public bool SimulateEscape( IActor otherActor )
    {
      throw new NotImplementedException();
    }

    public bool SimulateHit( IActor otherActor )
    {
      throw new NotImplementedException();
    }

    public Coord SimulateMove( SimulationSnapshot snapshot )
    {
      throw new NotImplementedException();
    }

    public void TakeDamage( int damage )
    {
      throw new NotImplementedException();
    }
  }

  #endregion

  [Fact]
  public void Test_Class_Creation()
  {
    var partyManager = new PartyManager();
    Assert.NotNull( partyManager );
  }

  [Fact]
  public void Test_Empty_Actor_No_Party()
  {
    MockActor actor = new MockActor();
    Assert.False( actor.IsInParty() );
  }

  [Fact]
  public void Test_Create_Party()
  {
    MockActor actor1 = new MockActor();
    MockActor actor2 = new MockActor();

    var partyManager = new PartyManager();
    Assert.NotNull( partyManager );

    Assert.False( actor1.IsInParty() );
    Assert.False( actor2.IsInParty() );

    partyManager.CreateParty( actor1, actor2 );

    Assert.True( actor1.IsInParty() );
    Assert.True( actor2.IsInParty() );
  }

  [Fact]
  public void Test_Join_Party()
  {
    MockActor actor1 = new MockActor();
    MockActor actor2 = new MockActor();
    MockActor actor3 = new MockActor();

    var partyManager = new PartyManager();
    Assert.NotNull( partyManager );

    Assert.False( actor1.IsInParty() );
    Assert.False( actor2.IsInParty() );

    partyManager.CreateParty( actor1, actor2 );

    Assert.True( actor1.IsInParty() );
    Assert.True( actor2.IsInParty() );

    partyManager.JoinParty( actor3, actor2.PartyId );

    Assert.True( actor3.IsInParty() );
  }

  [Fact]
  public void Test_Leave_Party()
  {
    MockActor actor1 = new MockActor();
    MockActor actor2 = new MockActor();

    var partyManager = new PartyManager();
    Assert.NotNull( partyManager );

    Assert.False( actor1.IsInParty() );
    Assert.False( actor2.IsInParty() );

    partyManager.CreateParty( actor1, actor2 );

    Assert.True( actor1.IsInParty() );
    Assert.True( actor2.IsInParty() );

    partyManager.LeaveParty(  actor2 );

    Assert.True( actor1.IsInParty() );
    Assert.False( actor2.IsInParty() );
  }

  [Fact]
  public void Test_Disband_Party()
  {
    MockActor actor1 = new MockActor();
    MockActor actor2 = new MockActor();

    var partyManager = new PartyManager();
    Assert.NotNull( partyManager );

    Assert.False( actor1.IsInParty() );
    Assert.False( actor2.IsInParty() );

    partyManager.CreateParty( actor1, actor2 );

    Assert.True( actor1.IsInParty() );
    Assert.True( actor2.IsInParty() );

    partyManager.DisbandParty( new List<IActor>() { actor1 , actor2 } );

    Assert.False( actor1.IsInParty() );
    Assert.False( actor2.IsInParty() );
  }

  [Fact]
  public void Test_Disband_Party_Fail()
  {
    MockActor actor1 = new MockActor();
    MockActor actor2 = new MockActor();

    var partyManager = new PartyManager();
    Assert.NotNull( partyManager );

    Assert.False( actor1.IsInParty() );
    Assert.False( actor2.IsInParty() );

    partyManager.CreateParty( actor1, actor2 );

    Assert.True( actor1.IsInParty() );
    Assert.True( actor2.IsInParty() );

    Assert.Throws<ArgumentException>( () => partyManager.DisbandParty( new List<IActor>() ) );
  }


  [Fact]
  public void Test_Merge_Party()
  {
    MockActor actor1 = new MockActor();
    MockActor actor2 = new MockActor();
    MockActor actor3 = new MockActor();
    MockActor actor4 = new MockActor();

    var partyManager = new PartyManager();
    Assert.NotNull( partyManager );

    Assert.False( actor1.IsInParty() );
    Assert.False( actor2.IsInParty() );

    partyManager.CreateParty( actor1, actor2 );

    Assert.True( actor1.IsInParty() );
    Assert.True( actor2.IsInParty() );

    partyManager.CreateParty( actor3, actor4 );

    Assert.True( actor3.IsInParty() );
    Assert.True( actor4.IsInParty() );

    partyManager.MergeParties( new List<IActor>() { actor1, actor2 }, new List<IActor>() { actor3, actor4 } );

    Assert.True( actor1.PartyId == actor3.PartyId );
    Assert.True( actor1.PartyId == actor4.PartyId );

    Assert.True( actor2.PartyId == actor3.PartyId );
    Assert.True( actor2.PartyId == actor4.PartyId );
  }

  [Fact]
  public void Test_Merge_Party_Fail()
  {
    MockActor actor1 = new MockActor();
    MockActor actor2 = new MockActor();
    MockActor actor3 = new MockActor();
    MockActor actor4 = new MockActor();

    var partyManager = new PartyManager();
    Assert.NotNull( partyManager );

    Assert.False( actor1.IsInParty() );
    Assert.False( actor2.IsInParty() );

    partyManager.CreateParty( actor1, actor2 );

    Assert.True( actor1.IsInParty() );
    Assert.True( actor2.IsInParty() );

    partyManager.CreateParty( actor3, actor4 );

    Assert.True( actor3.IsInParty() );
    Assert.True( actor4.IsInParty() );

    Assert.Throws<ArgumentException>( () => partyManager.MergeParties( new List<IActor>(), new List<IActor>() ) );
    Assert.Throws<ArgumentException>( () => partyManager.MergeParties( new List<IActor>() { actor1 } , new List<IActor>() ) );
  }
}
