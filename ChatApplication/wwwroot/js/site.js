$(document).ready(function () {
    $('input, select, textarea').on('input change', function () {
        const fieldName = $(this).attr('name');
        $(`[data-valmsg-for="${fieldName}"]`).text('');
    });
});
function previewImage() {
    const input = document.getElementById('image');
    const preview = document.getElementById('imagePreview');
    const file = input.files[0];

    if (file) {
        const validTypes = ['image/jpeg', 'image/png', 'image/gif', 'image/webp'];
        if (!validTypes.includes(file.type)) {
            alert('Please select a valid image file (jpg, png, gif, webp).');
            input.value = '';
            preview.style.display = 'none';
            return;
        }

        const reader = new FileReader();
        reader.onload = function (e) {
            preview.src = e.target.result;
            preview.style.display = 'block';
        };
        reader.readAsDataURL(file);
    }
    else {
        preview.src = "#";
        preview.style.display = 'none';
    }
}
function previewSelectedImage(event) {
    const input = event.target;
    const preview = document.getElementById("imagePrevieww");
    const file = input.files[0];

    if (file) {
        const validImageTypes = ['image/jpeg', 'image/png', 'image/gif', 'image/webp'];

        if (!validImageTypes.includes(file.type)) {
            alert('Only image files (jpg, png, gif, webp) are allowed.');
            input.value = '';
            preview.style.display = "none";
            return;
        }

        const reader = new FileReader();
        reader.onload = function (e) {
            preview.src = e.target.result;
            preview.style.display = "block";
        };
        reader.readAsDataURL(file);
    }
}
function Logout() {
    $.ajax({
        url: '/Account/Logout',
        type: 'GET',
        success: function () {
            sessionStorage.clear();
            window.location.href = '/Account/Login';
            window.location.reload();
        },
        error: function (xhr, status, error) {
            console.error('Error:', error);
        }
    });
}

let connection = new signalR.HubConnectionBuilder().withUrl("/chatHub", { withCredentials: true }).build();

const friendId = $("#friendId").val();
let chattingWithUserId = friendId;

const currentUserIdd = $("#currentUserIdd").val();
const currentUsername = $("#currentUsername").val();
const groupId = $("#groupId").val();
const groupName = $("#groupName").val();

$(function () {

    connection.start().then(function () {
        console.log("SignalR connected");
        loadPendingRequests();
        loadFriends();
        loadMyGroups();
        loadChatHistory();
        const friendId = $("#chatBox").data("user-id");
        if (friendId) {
            connection.invoke("CheckUserStatus", friendId);
        }
        if (groupId && groupName) {
            connection.invoke("JoinGroup", groupId)
            loadGroupChatHistory();
        }
    })
    .catch (function (err) {
        console.error("SignalR connection error:", err.toString());
    });

    connection.on("NewGroupCreated", function (groupId, groupName) {
        alert(`You have been added to a new group: ${groupName}`);
        loadMyGroups(); 
    });

    connection.on("AddedToGroup", function (groupId, groupName) {
        alert(`You have been added to group: ${groupName}`);
        loadMyGroups(); 
    });

    connection.on("YouWereRemovedFromGroup", function (groupId, groupName) {
        alert(`You were removed from group: ${groupName}.`);
        loadMyGroups(); 
    });

    $('#createGroupModal').on('show.bs.modal', function () {
        loadFriendsForGroupSelection();
    });

    $("#confirmCreateGroupBtn").on("click", function () {
        const groupName = $("#modalGroupName").val().trim();
        const selectedMembers = [];
        $(".groupMemberCheckbox:checked").each(function () {
            selectedMembers.push(parseInt($(this).val()));
        });

        if (!groupName) {
            alert("Please enter a group name.");
            return;
        }

        $.ajax({
            url: "/Dashboard/CreateGroup",
            type: "POST",
            contentType: "application/json",
            data: JSON.stringify({ groupName: groupName, memberUserIds: selectedMembers }),
            success: function (response) {
                alert(response.message);
                $('#createGroupModal').modal('hide');
                loadMyGroups(); 
            },
            error: function (xhr, status, error) {
                console.error("Error creating group:", xhr.responseText);
                alert("Error creating group: " + (xhr.responseJSON?.message || error));
            }
        });
    });

    $(document).on("click", ".groupItem", function () {
        const groupId = $(this).data("id");
        window.location.href = `/Chat/ChatWithGroup?groupId=${groupId}`;
    });

    connection.on("ReceiveGroupMessage", function (fromUserId, fromUsername, message, receivedGroupId) {
        if (receivedGroupId == groupId) {
            const sender = fromUserId == currentUserId ? "You" : fromUsername;
            const alignment = fromUserId == currentUserIdd ? "message-from-me" : "message-from-other";
            const msgHtml = `<div class="${alignment}"><b>${sender}:</b> ${message}</div>`;
            $("#groupChatBox").append(msgHtml);
            scrollGroupChatToBottom("groupChatBox");
        }
    });

    connection.on("ReceiveSystemMessageGroup", function (message) {
        $("#groupChatBox").append(`<div class="text-muted text-center small">${message}</div>`);
        scrollGroupChatToBottom("groupChatBox");
    });

    connection.on("YouWereRemovedFromGroup", function (removedGroupId, removedGroupName) {
        if (removedGroupId == groupId) {
            alert(`You were removed from the group: ${removedGroupName}. You will be redirected to the dashboard.`);
            window.location.href = "/Dashboard/Dashboard";
        }
    });

    $('#groupMessageInput').keypress(function (e) {
        if (e.which === 13 && !e.shiftKey) { 
            e.preventDefault(); 
            sendGroupMessage();
        }
    });

    $('#groupMembersModal').on('show.bs.modal', function () {
        loadCurrentGroupMembers();
        $("#addGroupMemberSearch").val('');
        $("#addableGroupMembersList").empty();
    });

    $(document).on("keyup", "#addGroupMemberSearch", function () {
        let searchTerm = $(this).val().trim();
        if (searchTerm.length === 0) {
            $("#addableGroupMembersList").html("");
            return;
        }
        searchFriendsToAdd(searchTerm, groupId);
    });

    connection.on("ReceiveUserStatus", function (userId, isOnline) {
        const badge = $("#chatFriendStatus");
        badge
            .removeClass("bg-success bg-secondary")
            .addClass(isOnline ? "bg-success" : "bg-secondary")
            .text(isOnline ? "Online" : "Offline");
    });

    connection.on("UserStatusChanged", function (userId, isOnline) {
        const frienditem = $(`.friendItem[data-id="${userId}"]`);
        if (frienditem.length > 0) {
            frienditem.find("span.badge")
                .removeClass("bg-success bg-secondary")
                .addClass(isOnline ? "bg-success" : "bg-secondary")
                .text(isOnline ? "Online" : "Offline");
        }
        const chatUserId = $("#chatBox").data("user-id"); 
        if (chatUserId == userId) {
            const badge = $("#chatFriendStatus");
            badge
                .removeClass("bg-success bg-secondary")
                .addClass(isOnline ? "bg-success" : "bg-secondary")
                .text(isOnline ? "Online" : "Offline");
        }
        const groupitem = $(`.groupmembers[data-id="${userId}"]`);
        if (groupitem.length > 0) {
            groupitem.find("span.badge")
                .removeClass("bg-success bg-secondary")
                .addClass(isOnline ? "bg-success" : "bg-secondary")
                .text(isOnline ? "Online" : "Offline");
        }
    });

    connection.on("ReceiveSystemMessage", function (message) {
        $("#chatBox").append(`<div class="text-muted text-center small">${message}</div>`);
    });

    connection.on("ReceiveMessage", function (friendId, message) {
        const currentUserId = $("#currentUserId").val();
        const friendName = $("#friendName").val(); 
        let sender = friendId == currentUserId ? "You" : friendName;
        const alignment = sender === "You" ? "text-end" : "text-start";
        const msgHtml = `<div class="${alignment}"><b>${sender}:</b> ${message}</div>`;
        $("#chatBox").append(msgHtml);
        scrollChatToBottom();
    });

    connection.on("UserTyping", function (fromUser) {
        $("#typingStatus").text(`${fromUser} is typing...`);
        setTimeout(() => $("#typingStatus").text(""), 2000);
    });

    connection.on("FriendRemoved", function (fromUserId) {
        //loadFriends();
        window.location.reload();
    });

    const emailSearch = sessionStorage.getItem("lastEmailSearch");

    if (emailSearch) {
        $("#emailSearch").val(emailSearch);
        performEmailSearch(emailSearch);
    }

    $("#emailSearch").on("keyup", function () {
        let email = $(this).val().trim();
        sessionStorage.setItem("lastEmailSearch", email);
        if (email.length === 0) {
            $("#searchResults").html(""); 
            return;
        }
        performEmailSearch(email);
    });

    $(document).on("click", ".addFriendBtn", function () {
        const $button = $(this);
        const id = $button.data("id");

        $.post("/Dashboard/SendRequest", { toUserId: id }, function () {
            $button.replaceWith(`<span class="badge bg-warning">Pending</span>`);
        });
    });

    $(document).on("click", ".removeFriendBtn", function () {
        const id = $(this).data("id");
        const action = $(this).data("action");
        let postData = {};

        if (action === "removeFriend") {
            postData.friendUserId = id;
        } else if (action === "removeRequest") {
            postData.requestId = id;     
        }
        $.post("/Dashboard/RemoveFriend", postData, function () {
            $("#emailSearch").trigger("keyup"); 
            loadPendingRequests();
            loadFriends(); 
        });
    });

    $(document).on("click", ".acceptRequestBtn", function () {
        const requestId = $(this).data("id");
        $.post("/Dashboard/AcceptRequest", { requestId: requestId }, function () {
            loadPendingRequests();
            loadFriends();
        });
    });

    const lastUsernameSearch = sessionStorage.getItem("lastUsernameSearch");
    if (lastUsernameSearch) {
        $("#usernameSearch").val(lastUsernameSearch);
        loadFriends(lastUsernameSearch);
    }
    else {
        loadFriends();
    }

    $("#usernameSearch").on("keyup", function () {
        let name = $(this).val().trim();
        sessionStorage.setItem("lastUsernameSearch", name);
        loadFriends(name);
    });

    $(document).on("click", ".friendItem", function () {
        const chatFriendId = $(this).data("id");
        window.location.href = `/Chat/ChatWithUser?chatFriendId=${chatFriendId}`;
    });

    $("#messageInput").on("input", function () {
        if (chattingWithUserId)
            connection.invoke("Typing", chattingWithUserId.toString());
    });
    $('#messageInput').keypress(function (e) {
        if (e.which === 13 && !e.shiftKey) { 
            e.preventDefault(); 
            sendMessage(); 
        }
    });
});

function performEmailSearch(email) {
    $.get("/Dashboard/Search?email=" + email, function (users) {
        let html = "";
        users.forEach(u => {
            let buttonHtml = "";

            if (u.action === "add") {
                buttonHtml = `<button class="btn btn-sm btn-success addFriendBtn" data-id="${u.userId}">Add</button>`;
            } else if (u.action === "pending") {
                buttonHtml = `<span class="badge bg-warning">Pending</span>`;
            } else if (u.action === "remove") {
                buttonHtml = `<button class="btn btn-sm btn-danger removeFriendBtn" data-id="${u.userId}" data-action="removeFriend">Remove</button>`;
            }

            html += `<li class="list-group-item d-flex justify-content-between align-items-center">
                        ${u.username}
                        ${buttonHtml}
                     </li>`;
        });
        $("#searchResults").html(html);
    });
}
function loadFriends(name = "") {
    let url = "/Dashboard/FriendList";
    if (name && name.trim() !== "") {
        url += "?username=" + encodeURIComponent(name.trim());
    }

    $.get(url, function (friends) {
        let html = "";
        friends.forEach(f => {
            let onlineBadge = f.isOnline
                ? '<span class="badge bg-success">Online</span>'
                : '<span class="badge bg-secondary">Offline</span>';

            html += `<div class="d-flex justify-content-between align-items-center mb-2">
                <li class="list-group-item flex-grow-1 friendItem" data-id="${f.userId}" data-name="${f.username}">
                    <span>${f.username} ${onlineBadge}</span>
                </li>
                <button class="btn btn-sm btn-danger ms-2 removeFriendBtn" data-id="${f.userId}" data-action="removeFriend">Remove</button>
             </div>`;
        });
        $("#friendList").html(html);
    });
}
function loadPendingRequests() {
    $.get("/Dashboard/GetPendingRequests", function (requests) {
        let html = "";
        requests.filter(r => r.status === "Pending").forEach(r => {
            html += `<li class="list-group-item d-flex justify-content-between align-items-center">
                        <span>${r.fromUsername}</span>
                        <button class="btn btn-sm btn-primary acceptRequestBtn" data-id="${r.requestId}">Accept</button>
                        <button class="btn btn-sm btn-danger removeFriendBtn" data-id="${r.requestId}" data-action="removeRequest">Remove</button>
                     </li>`;
        });
        $("#pendingRequests").html(html);
    });
}
function sendMessage() {
    const friendId = $("#friendId").val();
    const message = $("#messageInput").val();
    const file = $("#fileInput")[0].files[0];

    if (message && message.trim() !== '') {
        connection.invoke("SendMessage", friendId.toString(), message)
            .catch(function (err) {
                console.error(err.toString());
            });
        $("#messageInput").val('');
        scrollChatToBottom();
    }
    if (file) {
        if (file.size > 1024 * 1024 * 1024) {
            alert("File exceeds limit.");
            return;
        }

        const formData = new FormData();
        formData.append("file", file);

        $.ajax({
            url: "/Chat/Upload",
            type: "POST",
            processData: false,
            contentType: false,
            data: formData,
            success: function (res) {
                if (res.success) {
                    const fileLink = `<a href="${res.url}" target="_blank">Download File</a>`;
                    connection.invoke("SendMessage", friendId.toString(), fileLink)
                        .catch(function (err) {
                            console.error(err.toString());
                        });
                    $("#fileInput").val(""); 
                    $("#selectedFileName1").text(""); 
                    scrollChatToBottom();
                }
            }
        });
    }
}
function loadChatHistory() {
    const friendId = $("#friendId").val();
    const friendName = $("#friendName").val();
    $.get(`/Chat/GetMessages?userId=${friendId}`, function (data) {
        $("#chatBox").empty();
        data.forEach(msg => {
            const sender = msg.fromUserId == $("#currentUserId").val() ? "You" : friendName;
            const msgHtml = `<div class="${sender === 'You' ? 'text-end' : 'text-start'}"><b>${sender}:</b> ${msg.messageText}</div>`;
            $("#chatBox").append(msgHtml);
        });
        scrollChatToBottom();
    });
}
function scrollChatToBottom() {
    const chatBox = document.getElementById("chatBox");
    if (chatBox) {
        chatBox.scrollTop = chatBox.scrollHeight;
    }
}


function sendGroupMessage() {
    const message = $("#groupMessageInput").val();
    const file = $("#groupFileInput")[0].files[0];

    if (message && message.trim() !== '') {
        connection.invoke("SendGroupMessage", groupId.toString(), groupName, message)
            .catch(function (err) {
                console.error(err.toString());
            });
        $("#groupMessageInput").val('');
        scrollGroupChatToBottom("groupChatBox");
    }
    if (file) {
        if (file.size > 1024 * 1024 * 1024) { 
            alert("File exceeds limit.");
            return;
        }

        const formData = new FormData();
        formData.append("file", file);

        $.ajax({
            url: "/Chat/Upload",
            type: "POST",
            processData: false,
            contentType: false,
            data: formData,
            success: function (res) {
                if (res.success) {
                    const fileLink = `<a href="${res.url}" target="_blank">Download File</a>`;
                    connection.invoke("SendGroupMessage", groupName, fileLink)
                        .catch(function (err) {
                            console.error(err.toString());
                        });
                    $("#groupFileInput").val('');
                    $("#selectedFileName").text('');
                    scrollGroupChatToBottom("groupChatBox");
                } else {
                    alert(res.message);
                }
            },
            error: function (xhr, status, error) {
                console.error("File upload error:", xhr.responseText);
                alert("Error uploading file.");
            }
        });
    }
}

function loadGroupChatHistory() {
    $.get(`/Chat/GetGroupMessages?groupId=${groupId}`, function (data) {
        $("#groupChatBox").empty();
        data.forEach(msg => {
            const sender = msg.fromUserId == currentUserId ? "You" : msg.fromUsername;
            const alignment = sender === 'You' ? 'message-from-me' : 'message-from-other';
            const msgHtml = `<div class="${alignment}"><b>${sender}:</b> ${msg.messageText}</div>`;
            $("#groupChatBox").append(msgHtml);
        });
        scrollGroupChatToBottom("groupChatBox");
    });
}

function leaveGroup() {
    if (confirm(`Are you sure you want to leave the group '${groupName}'?`)) {
        $.post("/Dashboard/LeaveGroup", { groupId: groupId, groupName: groupName })
            .done(function (response) {
                alert(response.message);
                window.location.href = "/Dashboard/Dashboard";
            })
            .fail(function (xhr) {
                alert("Failed to leave group: " + (xhr.responseJSON?.message || xhr.statusText));
            });
    }
}

function loadCurrentGroupMembers() {
    $.get(`/Dashboard/GetGroupMembers?groupId=${groupId}`, function (response) {
        var members = response.members;
        var groupAdmin = response.groupAdmin;

        let html = "";
        members.forEach(member => {
            let onlineStatus = member.isOnline ? '<span class="badge bg-success">Online</span>' : '<span class="badge bg-secondary">Offline</span>';
            html += `<li class="list-group-item d-flex justify-content-between align-items-center groupmembers" data-id="${member.userId}" data-name="${member.username}">
                                <span>${member.username} ${onlineStatus}</span>
                                <div class="d-flex align-items-center gap-2">`;
            if (member.isAdmin == true) {
                html += `<span class="badge bg-primary">Admin</span>`;
            }
            if (member.userId != currentUserId && groupAdmin?.isAdmin == true) {
                html += `<div class="dropdown">
                            <i class="bi bi-three-dots-vertical" role="button" data-bs-toggle="dropdown" aria-expanded="false"></i>
                            <ul class="dropdown-menu">
                                <li><a class="dropdown-item text-danger" href="#" onclick="removeGroupMember(${groupId}, ${member.userId})">Remove from Group member</a></li>`;
                if (member.isAdmin != true) {
                    html += `<li><a class="dropdown-item text-primary" href="#" onclick="makeAdmin(${groupId}, ${member.userId})">Make Admin</a></li>`;
                }
                if (member.isAdmin == true) {
                    html += `<li><a class="dropdown-item text-danger" href="#" onclick="removeAdmin(${groupId}, ${member.userId})">Remove Admin</a></li>`;
                }
                html += `</ul></div>`;
            }
            html += `</div></li>`;
        });
        if (groupAdmin?.isAdmin == true) {
            html += `<h6>Add Members:</h6>
                <div class="mb-3">
                    <input type="text" id="addGroupMemberSearch" class="form-control mb-2" placeholder="Search friends by username/email to add...">
                    <ul id="addableGroupMembersList" class="list-group">
                    </ul>
                </div>`;
        }
        $("#currentGroupMembersList").html(html);
    });
}

function removeGroupMember(groupId, userIdToRemove) {
    if (confirm("Are you sure you want to remove this member from the group?")) {
        $.post("/Dashboard/RemoveGroupMember", { groupId: groupId, userIdToRemove: userIdToRemove }, function () {
            alert("Member removed.");
            loadCurrentGroupMembers(); 
        }).fail(function (xhr, status, error) {
            alert("Error removing member: " + (xhr.responseJSON?.message || error));
        });
    }
}

function searchFriendsToAdd(searchTerm, groupId) {
    $.get(`/Dashboard/FriendNotInGroup`, { username: searchTerm, groupId: groupId }, function (users) { 
        let html = "";
        users.forEach(u => {
            
            let isAlreadyMember = $("#currentGroupMembersList li").text().includes(u.email); 

            if (u.userId != currentUserId && !isAlreadyMember) {
                html += `<li class="list-group-item d-flex justify-content-between align-items-center">
                                    <span>${u.username}</span>
                                    <button class="btn btn-sm btn-success" onclick="addMemberToGroup(${groupId}, ${u.userId})">Add</button>
                                </li>`;
            }
        });
        $("#addableGroupMembersList").html(html);
    });
}

function addMemberToGroup(groupId, userIdToAdd) {
    $.post("/Dashboard/AddMemberToGroup", { groupId: groupId, userIdToAdd: userIdToAdd }, function () {
        alert("Member added to group.");
        loadCurrentGroupMembers(); 
        $("#addGroupMemberSearch").val(''); 
        $("#addableGroupMembersList").empty(); 
    }).fail(function (xhr, status, error) {
        alert("Error adding member: " + (xhr.responseJSON?.message || error));
    });
}

function loadMyGroups() {
    $.get("/Dashboard/GetMyGroups", function (groups) {
        let html = "";
        groups.forEach(g => {
            html += `<div class="d-flex justify-content-between align-items-center mb-2">
                        <li class="list-group-item flex-grow-1 groupItem" data-id="${g.groupId}" data-name="${g.groupName}">
                            <span>${g.groupName}</span>
                        </li>
                    </div>`;
        });
        $("#myGroupList").html(html);
    });
}

function loadFriendsForGroupSelection() {
    $.get("/Dashboard/FriendList", function (friends) {
        let html = "";
        friends.forEach(f => {
            html += `<div class="form-check">
                        <input class="form-check-input groupMemberCheckbox" type="checkbox" value="${f.userId}" id="member-${f.userId}">
                        <label class="form-check-label" for="member-${f.userId}">${f.username}</label>
                    </div>`;
        });
        $("#groupMembersList").html(html);
    });
}

function makeAdmin(groupId, userId) {
    $.post("/Dashboard/MakeAdmin", { groupId: groupId, userId: userId }, function () {
        loadCurrentGroupMembers();
    });
}
function removeAdmin(groupId, userId) {
    $.post("/Dashboard/RemoveAdmin", { groupId: groupId, userId: userId }, function () {
        loadCurrentGroupMembers();
    });
}
function scrollGroupChatToBottom(elementId) {
    const chatBox = document.getElementById(elementId);
    if (chatBox) {
        chatBox.scrollTop = chatBox.scrollHeight;
    }
}
function showFileName() {
    const input = document.getElementById("groupFileInput");
    const fileNameDiv = document.getElementById("selectedFileName");
    if (input.files.length > 0) {
        fileNameDiv.textContent = input.files[0].name;
    } else {
        fileNameDiv.textContent = '';
    }
}
function showFileName1() {
    const input = document.getElementById("fileInput");
    const fileNameDiv = document.getElementById("selectedFileName1");
    if (input.files.length > 0) {
        fileNameDiv.textContent = input.files[0].name;
    } else {
        fileNameDiv.textContent = '';
    }
}