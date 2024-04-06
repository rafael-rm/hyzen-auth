create table roles (
    id int auto_increment primary key,
    guid char(36) not null,
    name varchar(64) not null,
    created_at datetime default NOW() not null,
    updated_at datetime default NOW() not null on update NOW(),
    constraint guid_uindex unique (guid),
    constraint name_uindex unique (name)
);

create table users (
    id int auto_increment primary key,
    guid char(36) not null,
    name varchar(255) not null,
    email varchar(255) not null,
    password varchar(255) not null,
    is_active bit default b'1' not null,
    created_at datetime default NOW() not null,
    updated_at datetime default NOW() not null on update NOW(),
    constraint email_uindex unique (email),
    constraint guid_uindex unique (guid)
);

create table users_roles (
    id int auto_increment primary key,
    user_id int not null,
    role_id int not null,
    assigned_at datetime default NOW() not null,
    constraint users_roles_roles_id_fk foreign key (role_id) references roles (id),
    constraint users_roles_users_id_fk foreign key (user_id) references users (id)
);