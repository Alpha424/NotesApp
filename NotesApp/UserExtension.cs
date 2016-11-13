﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NotesApp
{
    public static class UserExtension
    {
        public static IEnumerable<notes> GetAvailableNotes(this users user, DBModelDataContext dataContext, bool includeShared)
        {
            List<notes> availableNotes = new List<notes>();
            availableNotes.AddRange(user.notes);
            if (includeShared)
            {
                IEnumerable<notes> sharedNotes = from note in dataContext.notes
                                  join sh in dataContext.shared on note.id equals sh.noteid
                                  where sh.sharedwith == user.id
                                  select note;
                availableNotes.AddRange(sharedNotes);
            }
            return availableNotes;
        }

        public static string GetUsernameByID(this int id, DBModelDataContext dataContext)
        {
            return dataContext.users.First(u => u.id == id).username;
        }
    }
}
