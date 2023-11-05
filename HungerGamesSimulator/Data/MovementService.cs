namespace HungerGamesSimulator.Data
{
    public class MovementService
    {
        public MovementResponse Move( MovementRequest request )
        {
            if ( !request.actorsToMove.Any() )
            {
                throw new ArgumentException( "There must be actors in order to move" );
            }

            var snapshot = request.snapshot;
            var partyLeader = request.actorsToMove[ Random.Shared.Next( 0, request.ActorsToMove.Count ) ];
            var lastLocation = partyLeader.Location;
            Coord wishLocation = partyLeader.SimulateMove( snapshot );

            // asure wish location is valid based on world information
            if ( wishLocation.X > snapshot.WorldWidth )
            {
                wishLocation.X = snapshot.WorldWidth;
            }
            else if ( wishLocation.X < 0 )
            {
                wishLocation.X = 0;
            }

            if ( wishLocation.Y > snapshot.WorldHeight )
            {
                wishLocation.Y = snapshot.WorldHeight;
            }
            else if ( wishLocation.Y < 0 )
            {
                wishLocation.Y = 0;
            }

            // set all actors to the same location
            foreach ( var actor in request.ActorsToMove )
            {
                actor.Location = wishLocation;
            }

            return new MovementResponse( lastLocation, wishLocation );
        }

    }

    public record MovementRequest( List<IActor> actorsToMove, SimulationSnapshot snapshot )
    {
        public List<IActor> ActorsToMove { get; } = actorsToMove;

        public SimulationSnapshot SimulationSnapshot { get; } = snapshot;
    }

    public record MovementResponse( Coord pastLocation, Coord newLocation )
    {
        public Coord PastLocation { get; } = pastLocation;
        public Coord NewLocation { get; } = newLocation;
    }

}
