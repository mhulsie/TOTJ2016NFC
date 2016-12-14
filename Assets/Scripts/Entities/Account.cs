using System;
using System.Collections.Generic;
using System.Text;

namespace totj3.Models
{
    public class Account : Model
    {
        public int accountID;
        public string location;
        public string nickName;
        public int roomID;
        public int vehicle;
        public int hat;

        public Account(string n, int v, int h)
        {
            this.nickName = n;
            this.vehicle = v;
            this.hat = h;
        }

        public Account() { }

        public Account(bool b)
        {
            if (b)
            {
                this.accountID = AccountState.accountID;
                this.location = AccountState.location;
                this.nickName = AccountState.nickName;
                this.roomID = AccountState.room;
                this.vehicle = AccountState.vehicle;
                this.hat = AccountState.hat;
            }
            else
            {
                accountID = 0;
            }
        }                                
                                                     
        public void PlayerToPlayerState()            
        {
            AccountState.accountID = this.accountID;
            AccountState.location = this.location;
            AccountState.nickName = this.nickName;
            AccountState.room = this.roomID;
            AccountState.vehicle = this.vehicle;
            AccountState.hat = this.hat;
        }

    }
}
