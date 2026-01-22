using Task_Flow.DataAccess.Abstract;
using Task_Flow.Entities.Models;

namespace Task_Flow.Business.Cocrete
{
    public class QuizService : IQuizService
    {
        private readonly IQuizDal dal;
        private readonly IUserDal ual;

        public QuizService(IQuizDal dal, IUserDal ual)
        {
            this.dal = dal;
            this.ual = ual;
        }

        public async Task Add(Quiz quiz)
        {
            await dal.Add(quiz);    
        }

        

        public async Task<List<Quiz>> Quizzes()
        {
            return await dal.GetAll();
        }

        public async Task<int> SpecialOccupationCount(string occupation)
        {
            var temp = await ual.GetAll(u => u.Occupation == occupation);

            // var list = await dal.GetAll();
            return temp.ToList().Count();
        }
    

        public async Task Update(Quiz quiz)
        {
            await  dal.Update(quiz);
        }
    }
}
