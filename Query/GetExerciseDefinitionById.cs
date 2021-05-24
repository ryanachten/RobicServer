using MediatR;
using RobicServer.Models;

namespace RobicServer.Query
{
    public class GetExerciseDefinitionById : IRequest<ExerciseDefinition> {

        public string ExerciseId { get; set; }
    }
}
