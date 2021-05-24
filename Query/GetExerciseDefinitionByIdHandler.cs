using MediatR;
using RobicServer.Data;
using RobicServer.Models;
using System.Threading;
using System.Threading.Tasks;

namespace RobicServer.Query
{
    public class GetExerciseDefinitionByIdHandler : IRequestHandler<GetExerciseDefinitionById, ExerciseDefinition>
    {
        private readonly IExerciseDefinitionRepository _definitionRepo;
        private readonly IExerciseRepository _exerciseRepo;

        public GetExerciseDefinitionByIdHandler(IUnitOfWork unitOfWork)
        {
            _definitionRepo = unitOfWork.ExerciseDefinitionRepo;
            _exerciseRepo = unitOfWork.ExerciseRepo;
        }

        public async Task<ExerciseDefinition> Handle(GetExerciseDefinitionById request, CancellationToken cancellationToken)
        {
            var exercise = await _definitionRepo.GetExerciseDefinition(request.ExerciseId);
            if(exercise != null)
                exercise.PersonalBest = _exerciseRepo.GetPersonalBest(exercise.Id);

            return exercise;
        }
    }
}
