using System.Collections.Generic;

namespace HungerGamesSimulator.Data
{
  /// <summary>
  /// Creates and manages tributes parties
  /// </summary>
  public class PartyService
  {
    private Dictionary<Guid, List<Guid>> partyData;

    public PartyService()
    {
      partyData = new Dictionary<Guid, List<Guid>>();
    }

    public void MergeParties( List<IActor> actorsInParty1, List<IActor> actorsInParty2 )
    {
      if ( !actorsInParty1.Any() || !actorsInParty2.Any() )
      {
        throw new ArgumentException( $"There must be actors in the lists in order to merge the parties." );
      }

      var party1 = actorsInParty1[ 0 ].PartyId;
      var party2 = actorsInParty2[ 0 ].PartyId;

      var newPartyId = Guid.NewGuid();
      var newActorIdList = partyData[ party1 ].Concat( partyData[ party2 ] ).ToList();
      partyData.Add( newPartyId, newActorIdList );
      partyData.Remove( party1 );
      partyData.Remove( party2 );

      var newParty = actorsInParty1.Concat( actorsInParty2 );

      foreach ( var actor in newParty )
      {
        actor.PartyId = newPartyId;
      }
    }

    public void JoinParty( IActor actor, Guid partyToJoin )
    {
      partyData[ partyToJoin ].Add( actor.ActorId );
      actor.PartyId = partyToJoin;
    }

    public void DisbandParty( List<IActor> actorsInParty )
    {
      if ( !actorsInParty.Any() )
      {
        throw new ArgumentException( $"You can't disband a party without any actors" );
      }

      partyData.Remove( actorsInParty[ 0 ].PartyId );
      foreach ( var actor in actorsInParty )
      {
        actor.PartyId = Guid.Empty;
      }
    }

    public void LeaveParty( IActor actorToLeave )
    {
      var partyMembers = partyData[ actorToLeave.PartyId ];
      partyMembers.Remove( actorToLeave.ActorId );
      actorToLeave.PartyId = Guid.Empty;
    }

    public void CreateParty( IActor actor, IActor otherActor )
    {
      // generate data
      var partyId = Guid.NewGuid();
      var partyMembers = new List<Guid>() { actor.ActorId, otherActor.ActorId };

      // create party
      partyData.Add( partyId, partyMembers );
      actor.PartyId = partyId;
      otherActor.PartyId = partyId;
    }

  }
}
