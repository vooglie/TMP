﻿
namespace TMP.BLL.Services.Exercise
{    
    using DAL.Repositories;
    using System.Linq;
    using Domain.Entities.Exercises;
    using Domain.Exceptions.Exercise;
    using System;
    using System.Collections.Generic;
    using System.Linq.Expressions;

    public class ExerciseTypeService
    {
        private readonly string _dbKey;        
        
        public ExerciseTypeService(string dbKey)
        {
            _dbKey = dbKey;
        }

        public IEnumerable<ExerciseType> GetAll()
        {
            using (var repo = new ExerciseTypeRepository(_dbKey))
            {
                return repo.FindAll().ToList();
            }
        }        

        public void AddNewExercise(ExerciseType exercise)
        {
            if (exercise == null || string.IsNullOrEmpty(exercise.ExerciseName) || exercise.MetricType == Domain.Entities.MetricType.ERROR)
                throw new InvalidExerciseType();

            using (var repo = new ExerciseTypeRepository(_dbKey))
            {
                var matcingExercises = repo.Find(dbExc => 
                    dbExc.ExerciseName.Equals(exercise.ExerciseName.Trim(), StringComparison.OrdinalIgnoreCase)
                    &&
                    dbExc.MetricType == exercise.MetricType
                    );
                if (matcingExercises != null && matcingExercises.Count() > 0)
                    throw new ExerciseTypeAlreadyExists();

                exercise.Created = DateTime.Now;
                exercise.Modified = DateTime.Now;
                exercise.CreatedBy = null; // todo

                repo.Create(exercise);
                repo.Save();
            }            
        }

        public bool ExerciseExists(ExerciseType exercise)
        {
            if (exercise == null || string.IsNullOrEmpty(exercise.ExerciseName)) return false;
            using(var repo = new ExerciseTypeRepository(_dbKey))
            {
                var matcingExercises = repo.Find(dbExc => 
                    dbExc.ExerciseName.Equals(exercise.ExerciseName.Trim(), StringComparison.OrdinalIgnoreCase)
                    &&
                    dbExc.MetricType == exercise.MetricType
                    );
                if (matcingExercises != null && matcingExercises.Count() > 0)
                    return true;
            }
            return false;
        }
        
    }
}
