
----truncate table [FriendRequests]
----truncate table [FriendList]
----truncate table [Groups]
----truncate table [GroupMembers]
----truncate table [Messages]

--ALTER TABLE FriendRequests
--ADD CONSTRAINT fk_FriendRequests_FromUserId_Users_UserId
--FOREIGN KEY (FromUserId) REFERENCES Users(UserId);
----ALTER TABLE [FriendRequests] DROP CONSTRAINT fk_FriendRequests_FromUserId_Users_UserId;

--ALTER TABLE FriendRequests
--ADD CONSTRAINT fk_FriendRequests_ToUserId_Users_UserId
--FOREIGN KEY (ToUserId) REFERENCES Users(UserId);
----ALTER TABLE [FriendRequests] DROP CONSTRAINT fk_FriendRequests_ToUserId_Users_UserId;

--ALTER TABLE FriendList
--ADD CONSTRAINT fk_FriendList_UserId_Users_UserId
--FOREIGN KEY (UserId) REFERENCES Users(UserId);
----ALTER TABLE [FriendList] DROP CONSTRAINT fk_FriendList_UserId_Users_UserId;

--ALTER TABLE FriendList
--ADD CONSTRAINT fk_FriendList_FriendUserId_Users_UserId
--FOREIGN KEY (FriendUserId) REFERENCES Users(UserId);
----ALTER TABLE [FriendList] DROP CONSTRAINT fk_FriendList_FriendUserId_Users_UserId;

--ALTER TABLE Groups
--ADD CONSTRAINT fk_Groups_CreatedBy_Users_UserId
--FOREIGN KEY (CreatedBy) REFERENCES Users(UserId);
----ALTER TABLE [Groups] DROP CONSTRAINT fk_Groups_CreatedBy_Users_UserId;

--ALTER TABLE GroupMembers
--ADD CONSTRAINT fk_GroupMembers_GroupId_Groups_GroupId
--FOREIGN KEY (GroupId) REFERENCES Groups(GroupId);
----ALTER TABLE [GroupMembers] DROP CONSTRAINT fk_GroupMembers_GroupId_Groups_GroupId;

--ALTER TABLE GroupMembers
--ADD CONSTRAINT fk_GroupMembers_UserId_Users_UserId
--FOREIGN KEY (UserId) REFERENCES Users(UserId);
----ALTER TABLE [GroupMembers] DROP CONSTRAINT fk_GroupMembers_UserId_Users_UserId;

--ALTER TABLE Messages
--ADD CONSTRAINT fk_Messages_FromUserId_Users_UserId
--FOREIGN KEY (FromUserId) REFERENCES Users(UserId);
----ALTER TABLE [Messages] DROP CONSTRAINT fk_Messages_FromUserId_Users_UserId;

--ALTER TABLE Messages
--ADD CONSTRAINT fk_Messages_ToUserId_Users_UserId
--FOREIGN KEY (ToUserId) REFERENCES Users(UserId);
----ALTER TABLE [Messages] DROP CONSTRAINT fk_Messages_ToUserId_Users_UserId;

--ALTER TABLE Messages
--ADD CONSTRAINT fk_Messages_GroupId_Groups_GroupId
--FOREIGN KEY (GroupId) REFERENCES Groups(GroupId);
----ALTER TABLE [Messages] DROP CONSTRAINT fk_Messages_GroupId_Groups_GroupId;

select * from [dbo].[Users]
select * from [dbo].[FriendList]
select * from [dbo].[FriendRequests]
select * from [dbo].[Groups]
select * from [dbo].[GroupMembers]
select * from [dbo].[Messages]
