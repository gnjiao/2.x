namespace Vins.ML.Domain
{
    public class RobotResetCommandMqRequest
    {
        public int Index { get; set; }

        public RobotResetCommandMqRequest()
        {
        }

        public RobotResetCommandMqRequest(int index)
        {
            Index = index;
        }
    }

    public class RobotStartCommandMqRequest
    {
        public int Index { get; set; }

        public RobotStartCommandMqRequest()
        {
        }

        public RobotStartCommandMqRequest(int index)
        {
            Index = index;
        }
    }

    public class RobotStopCommandMqRequest
    {
        public int Index { get; set; }

        public RobotStopCommandMqRequest()
        {
        }

        public RobotStopCommandMqRequest(int index)
        {
            Index = index;
        }
    }

    public class StationStartCommandMqRequest
    {
        public int Index { get; set; }

        public StationStartCommandMqRequest()
        {
        }

        public StationStartCommandMqRequest(int index)
        {
            Index = index;
        }
    }

    public class StationResetCommandMqRequest
    {
        public int Index { get; set; }

        public StationResetCommandMqRequest()
        {
        }

        public StationResetCommandMqRequest(int index)
        {
            Index = index;
        }
    }

//    public class ForceWorkpieceLocatingReadyCommandMqRequest
//    {
//        public int Index { get; set; }
//
//        public ForceWorkpieceLocatingReadyCommandMqRequest()
//        {
//        }
//
//        public ForceWorkpieceLocatingReadyCommandMqRequest(int index)
//        {
//            Index = index;
//        }
//    }

    public class SetWorkpieceLocatingReadyPlcEventMqRequest
    {
        public int Index { get; set; }

        public SetWorkpieceLocatingReadyPlcEventMqRequest()
        {
        }

        public SetWorkpieceLocatingReadyPlcEventMqRequest(int index)
        {
            Index = index;
        }
    }

    public class ResetWorkpieceLocatingReadyPlcEventMqRequest
    {
        public int Index { get; set; }

        public ResetWorkpieceLocatingReadyPlcEventMqRequest()
        {
        }

        public ResetWorkpieceLocatingReadyPlcEventMqRequest(int index)
        {
            Index = index;
        }
    }

    public class SetAllMeasureDataProcessedAppEventMqRequest
    {
        public int Index { get; set; }

        public SetAllMeasureDataProcessedAppEventMqRequest()
        {
        }

        public SetAllMeasureDataProcessedAppEventMqRequest(int index)
        {
            Index = index;
        }
    }
}