using HungerGamesSimulator.Data;

namespace HungerGamesTests;

public class PartyServiceTests
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

    public IWeapon Weapon => throw new NotImplementedException();

    public ActorStates GetState()
    {
      throw new NotImplementedException();
    }

    public void GiveWeapon( IWeapon weapon )
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

    public void SetLocation( Coord location )
    {
      throw new NotImplementedException();
    }

    public bool SimulateEscape( IActor actor )
    {
      throw new NotImplementedException();
    }

    public bool SimulateHit( IActor actor )
    {
      throw new NotImplementedException();
    }

    public Coord SimulateMove()
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
    var partyService = new PartyService();
    Assert.NotNull( partyService );
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

    var partyService = new PartyService();
    Assert.NotNull( partyService );

    Assert.False( actor1.IsInParty() );
    Assert.False( actor2.IsInParty() );

    partyService.CreateParty( actor1, actor2 );

    Assert.True( actor1.IsInParty() );
    Assert.True( actor2.IsInParty() );
  }

  [Fact]
  public void Test_Join_Party()
  {
    MockActor actor1 = new MockActor();
    MockActor actor2 = new MockActor();
    MockActor actor3 = new MockActor();

    var partyService = new PartyService();
    Assert.NotNull( partyService );

    Assert.False( actor1.IsInParty() );
    Assert.False( actor2.IsInParty() );

    partyService.CreateParty( actor1, actor2 );

    Assert.True( actor1.IsInParty() );
    Assert.True( actor2.IsInParty() );

    partyService.JoinParty( actor3, actor2.PartyId );

    Assert.True( actor3.IsInParty() );
  }

  [Fact]
  public void Test_Leave_Party()
  {
    MockActor actor1 = new MockActor();
    MockActor actor2 = new MockActor();

    var partyService = new PartyService();
    Assert.NotNull( partyService );

    Assert.False( actor1.IsInParty() );
    Assert.False( actor2.IsInParty() );

    partyService.CreateParty( actor1, actor2 );

    Assert.True( actor1.IsInParty() );
    Assert.True( actor2.IsInParty() );

    partyService.LeaveParty(  actor2 );

    Assert.True( actor1.IsInParty() );
    Assert.False( actor2.IsInParty() );
  }

  [Fact]
  public void Test_Disband_Party()
  {
    MockActor actor1 = new MockActor();
    MockActor actor2 = new MockActor();

    var partyService = new PartyService();
    Assert.NotNull( partyService );

    Assert.False( actor1.IsInParty() );
    Assert.False( actor2.IsInParty() );

    partyService.CreateParty( actor1, actor2 );

    Assert.True( actor1.IsInParty() );
    Assert.True( actor2.IsInParty() );

    partyService.DisbandParty( new List<IActor>() { actor1 , actor2 } );

    Assert.False( actor1.IsInParty() );
    Assert.False( actor2.IsInParty() );
  }

  [Fact]
  public void Test_Disband_Party_Fail()
  {
    MockActor actor1 = new MockActor();
    MockActor actor2 = new MockActor();

    var partyService = new PartyService();
    Assert.NotNull( partyService );

    Assert.False( actor1.IsInParty() );
    Assert.False( actor2.IsInParty() );

    partyService.CreateParty( actor1, actor2 );

    Assert.True( actor1.IsInParty() );
    Assert.True( actor2.IsInParty() );

    Assert.Throws<ArgumentException>( () => partyService.DisbandParty( new List<IActor>() ) );
  }


  [Fact]
  public void Test_Merge_Party()
  {
    MockActor actor1 = new MockActor();
    MockActor actor2 = new MockActor();
    MockActor actor3 = new MockActor();
    MockActor actor4 = new MockActor();

    var partyService = new PartyService();
    Assert.NotNull( partyService );

    Assert.False( actor1.IsInParty() );
    Assert.False( actor2.IsInParty() );

    partyService.CreateParty( actor1, actor2 );

    Assert.True( actor1.IsInParty() );
    Assert.True( actor2.IsInParty() );

    partyService.CreateParty( actor3, actor4 );

    Assert.True( actor3.IsInParty() );
    Assert.True( actor4.IsInParty() );

    partyService.MergeParties( new List<IActor>() { actor1, actor2 }, new List<IActor>() { actor3, actor4 } );

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

    var partyService = new PartyService();
    Assert.NotNull( partyService );

    Assert.False( actor1.IsInParty() );
    Assert.False( actor2.IsInParty() );

    partyService.CreateParty( actor1, actor2 );

    Assert.True( actor1.IsInParty() );
    Assert.True( actor2.IsInParty() );

    partyService.CreateParty( actor3, actor4 );

    Assert.True( actor3.IsInParty() );
    Assert.True( actor4.IsInParty() );

    Assert.Throws<ArgumentException>( () => partyService.MergeParties( new List<IActor>(), new List<IActor>() ) );
    Assert.Throws<ArgumentException>( () => partyService.MergeParties( new List<IActor>() { actor1 } , new List<IActor>() ) );
  }
}
