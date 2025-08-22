use test;

drop table if exists user_token;
drop table if exists users;
drop table if exists  roles;
drop table if exists  user_types;
drop table if exists  adreses;
drop table if exists role_types;
drop table if exists user_types;
drop table if exists tokens;

create table tokens (
	id int(10) NOT NULL,
    name varchar(50),
    PRIMARY KEY (id)
);

insert into tokens (id, name) values(1, 'tokens 1');
insert into tokens (id, name) values(2, 'tokens 2');
insert into tokens (id, name) values(3, 'tokens 3');

create table role_types (
	id int(10) NOT NULL,
    name varchar(50),
    PRIMARY KEY (id)
);

insert into role_types (id, name) values(1, 'Gost');
insert into role_types (id, name) values(2, 'User');
insert into role_types (id, name) values(3, 'Admin');

create table roles (
	id int(10) NOT NULL,
    name varchar(50),
    role_type_id int(10),
    PRIMARY KEY (id),
    FOREIGN KEY (role_type_id) REFERENCES role_types(id)
);

insert into roles (id, name, role_type_id) values(1, 'Гость', 1);
insert into roles (id, name, role_type_id) values(2, 'Пользователь', 1);
insert into roles (id, name, role_type_id) values(3, 'Админ', 2);

create table user_types (
	id int(10) NOT NULL,
    name varchar(50),
    PRIMARY KEY (id)
);

insert into user_types (id, name) values(1, 'None');
insert into user_types (id, name) values(2, 'Simple');
insert into user_types (id, name) values(3, 'No Simple');

create table users (
	id int(10) NOT NULL,
    name varchar(50),
    role_id int(10),
    role2_id int(10),
    user_type int(10),
    user_id int(10),
    PRIMARY KEY (id),
    FOREIGN KEY (role_id) REFERENCES roles(id),
    FOREIGN KEY (role2_id) REFERENCES roles(id),
    FOREIGN KEY (user_type) REFERENCES user_types(id),
    FOREIGN KEY (user_id) REFERENCES users(id)
);

insert into users (id, name, role_id, role2_id, user_type, user_id) values(1, 'Alex 1', 1, 2, 1, null);
insert into users (id, name, role_id, role2_id, user_type, user_id) values(2, 'Alex 2', 2, 1, 2, 1);
insert into users (id, name, role_id, role2_id, user_type, user_id) values(3, 'Alex 3', 2, 1, 3, 2);
insert into users (id, name, role_id, role2_id, user_type, user_id) values(4, 'Alex 4', 3, 3, 1, 3);

create table user_token(
	id int(10) NOT NULL,
    user_id int(10),
    token_id int(10),
	PRIMARY KEY (id),
    FOREIGN KEY (user_id) REFERENCES users(id),
    FOREIGN KEY (token_id) REFERENCES tokens(id)
);

insert into user_token (id, user_id, token_id) values(1, 1, 1);
insert into user_token (id, user_id, token_id) values(2, 1, 2);
insert into user_token (id, user_id, token_id) values(3, 1, 3);
insert into user_token (id, user_id, token_id) values(4, 2, 1);
insert into user_token (id, user_id, token_id) values(5, 2, 2);
insert into user_token (id, user_id, token_id) values(6, 2, 3);
insert into user_token (id, user_id, token_id) values(7, 3, 1);
insert into user_token (id, user_id, token_id) values(8, 3, 2);
insert into user_token (id, user_id, token_id) values(9, 3, 3);