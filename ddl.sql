create table groups
(
    id          serial
        primary key,
    guid        uuid                                not null
        constraint groups_guid_uindex
            unique,
    name        varchar(64)                         not null
        constraint groups_name_uindex
            unique,
    description varchar(255)                        not null,
    created_at  timestamp default CURRENT_TIMESTAMP not null,
    updated_at  timestamp default CURRENT_TIMESTAMP not null
);

alter table groups
    owner to hyzen;

create table roles
(
    id          serial
        primary key,
    guid        uuid                                not null
        constraint roles_guid_uindex
            unique,
    name        varchar(64)                         not null
        constraint roles_name_uindex
            unique,
    description varchar(255)                        not null,
    created_at  timestamp default CURRENT_TIMESTAMP not null,
    updated_at  timestamp default CURRENT_TIMESTAMP not null
);

alter table roles
    owner to hyzen;

create table groups_roles
(
    id          serial
        primary key,
    group_id    integer                             not null
        references groups
            on delete cascade,
    role_id     integer                             not null
        references roles
            on delete cascade,
    assigned_at timestamp default CURRENT_TIMESTAMP not null
);

alter table groups_roles
    owner to hyzen;

create table users
(
    id            serial
        primary key,
    guid          uuid                                not null
        constraint users_guid_uindex
            unique,
    name          varchar(255)                        not null,
    email         varchar(255)                        not null
        constraint users_email_uindex
            unique,
    password      varchar(255)                        not null,
    is_active     boolean   default true              not null,
    last_login_at timestamp,
    created_at    timestamp default CURRENT_TIMESTAMP not null,
    updated_at    timestamp default CURRENT_TIMESTAMP not null
);

alter table users
    owner to hyzen;

create table users_groups
(
    id          serial
        primary key,
    user_id     integer                             not null
        references users
            on delete cascade,
    group_id    integer                             not null
        references groups
            on delete cascade,
    assigned_at timestamp default CURRENT_TIMESTAMP not null
);

alter table users_groups
    owner to hyzen;

create table users_roles
(
    id          serial
        primary key,
    user_id     integer                             not null
        references users
            on delete cascade,
    role_id     integer                             not null
        references roles
            on delete cascade,
    assigned_at timestamp default CURRENT_TIMESTAMP not null
);

alter table users_roles
    owner to hyzen;

create table verification_code
(
    id         serial
        primary key,
    code       varchar(12)                         not null,
    type       integer                             not null,
    created_at timestamp default CURRENT_TIMESTAMP not null,
    expires_at timestamp                           not null,
    used_at    timestamp,
    user_id    integer                             not null
        references users
            on delete cascade
);

alter table verification_code
    owner to hyzen;

create index verification_code_created_at_index
    on verification_code (created_at desc);

create table events
(
    id          serial
        primary key,
    user_id     integer not null
        constraint fk_user_event
            references users
            on delete cascade,
    event_type  integer not null,
    created_at  timestamp default CURRENT_TIMESTAMP,
    description text
);

alter table events
    owner to hyzen;

