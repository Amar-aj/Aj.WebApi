PGDMP     6    6                |            user_management    15.3    15.3 6    H           0    0    ENCODING    ENCODING        SET client_encoding = 'UTF8';
                      false            I           0    0 
   STDSTRINGS 
   STDSTRINGS     (   SET standard_conforming_strings = 'on';
                      false            J           0    0 
   SEARCHPATH 
   SEARCHPATH     8   SELECT pg_catalog.set_config('search_path', '', false);
                      false            K           1262    49152    user_management    DATABASE     �   CREATE DATABASE user_management WITH TEMPLATE = template0 ENCODING = 'UTF8' LOCALE_PROVIDER = libc LOCALE = 'English_United States.1252';
    DROP DATABASE user_management;
                postgres    false            �            1255    57350 /   fun_identity_roles_pagination(integer, integer)    FUNCTION     �  CREATE FUNCTION public.fun_identity_roles_pagination(page_number integer, page_size integer) RETURNS json
    LANGUAGE plpgsql
    AS $$
DECLARE
    total_count integer;
    role_data JSON := '[]';
    total_pages integer;
BEGIN
    -- Count the total number of roles
    SELECT COUNT(*) INTO total_count FROM m_roles WHERE deleted_on IS NULL;

    -- Calculate the total number of pages
    total_pages := CEIL(total_count::numeric / page_size);

    -- Retrieve paginated role data
    role_data := (
        SELECT json_agg(json_build_object(
            'role_id', r.role_id,
            'role_name', r.role_name,
            'role_description', r.role_description,
            'is_active', r.is_active
        ))
        FROM (
            SELECT role_id, role_name, role_description, is_active
            FROM m_roles WHERE deleted_on IS NULL
            ORDER BY updated_on DESC
            LIMIT page_size
            OFFSET (page_number - 1) * page_size
        ) AS r
    );

    -- Construct the result object
    RETURN json_build_object(
        'data', role_data,
        'total_pages', total_pages,
        'page_number', page_number,
        'page_size', page_size,
        'message', 'Role details retrieved successfully'
    );
END;
$$;
 \   DROP FUNCTION public.fun_identity_roles_pagination(page_number integer, page_size integer);
       public          postgres    false            �            1255    57353 7   fun_identity_roles_pagination(integer, integer, bigint)    FUNCTION     <  CREATE FUNCTION public.fun_identity_roles_pagination(page_number integer, page_size integer, filter_role_id bigint) RETURNS json
    LANGUAGE plpgsql
    AS $$
DECLARE
    total_count integer;
    role_data JSON := '[]';
    total_pages integer;
BEGIN
    -- Count the total number of roles
    SELECT COUNT(*) INTO total_count FROM m_roles WHERE deleted_on IS NULL;

    -- Calculate the total number of pages
    total_pages := CEIL(total_count::numeric / page_size);

    -- Retrieve paginated role data
    role_data := (
        SELECT json_agg(json_build_object(
            'role_id', r.role_id,
            'role_name', r.role_name,
            'role_description', r.role_description,
            'is_active', r.is_active
        ))
        FROM (
            SELECT role_id, role_name, role_description, is_active
            FROM m_roles 
			WHERE (filter_role_id IS NULL OR role_id = filter_role_id) AND deleted_on IS NULL
            ORDER BY updated_on DESC
            LIMIT page_size
            OFFSET (page_number - 1) * page_size
        ) AS r
    );

    -- Construct the result object
    RETURN json_build_object(
        'data', role_data,
        'total_pages', total_pages,
        'page_number', page_number,
        'page_size', page_size,
        'message', 'Role details retrieved successfully'
    );
END;
$$;
 s   DROP FUNCTION public.fun_identity_roles_pagination(page_number integer, page_size integer, filter_role_id bigint);
       public          postgres    false            �            1255    57351 -   fun_identity_user_password(character varying)    FUNCTION     �  CREATE FUNCTION public.fun_identity_user_password(p_email character varying) RETURNS character varying
    LANGUAGE plpgsql
    AS $$
DECLARE
    user_password VARCHAR(255);
BEGIN
    -- Retrieve the password for the given username
    SELECT password INTO user_password
    FROM t_users 
    WHERE email = p_email  AND deleted_on IS NULL ;

    -- Return the password
    RETURN user_password;
END;
$$;
 L   DROP FUNCTION public.fun_identity_user_password(p_email character varying);
       public          postgres    false            �            1255    57367 J   fun_identity_users_pagination(integer, integer, character varying, bigint)    FUNCTION     �  CREATE FUNCTION public.fun_identity_users_pagination(page_number integer, page_size integer, filter_email character varying DEFAULT NULL::character varying, filter_user_id bigint DEFAULT NULL::bigint) RETURNS json
    LANGUAGE plpgsql
    AS $$
DECLARE
    total_count integer;
    user_data JSON := '[]';
    total_pages integer;
BEGIN
    -- Count the total number of users
    SELECT COUNT(*) INTO total_count FROM t_users WHERE deleted_on IS NULL;

    -- Calculate the total number of pages
    total_pages := CEIL(total_count::numeric / page_size);

    -- Retrieve paginated user data
    user_data := (
        SELECT json_agg(json_build_object(
            'user_id', u.user_id,
            'username', u.username,
            'email', u.email,
            'is_active', u.is_active
        ))
        FROM (
            SELECT user_id, username, email, is_active
            FROM t_users
            WHERE (filter_email IS NULL OR email = filter_email)
                AND (filter_user_id IS NULL OR user_id = filter_user_id)
                AND deleted_on IS NULL
            ORDER BY updated_on DESC
            LIMIT page_size
            OFFSET (page_number - 1) * page_size
        ) AS u
    );

    -- Construct the result object
    RETURN json_build_object(
        'data', user_data,
        'total_pages', total_pages,
        'page_number', page_number,
        'page_size', page_size,
        'message', 'User details retrieved successfully'
    );
END;
$$;
 �   DROP FUNCTION public.fun_identity_users_pagination(page_number integer, page_size integer, filter_email character varying, filter_user_id bigint);
       public          postgres    false            �            1255    49419    generate_primary_key()    FUNCTION     �  CREATE FUNCTION public.generate_primary_key() RETURNS bigint
    LANGUAGE plpgsql
    AS $$
DECLARE
    pk_part text;
    seq_val bigint;
BEGIN
    -- Extract the current date and time components
    pk_part := to_char(CURRENT_TIMESTAMP, 'YYYYMMDDHH24MISS');

    -- Get the next value from the sequence
    SELECT nextval('row_sequence') INTO seq_val;
    
    -- Concatenate the sequence value to the datetime component
    pk_part := pk_part || seq_val::text;

    RETURN pk_part::bigint;
END;
$$;
 -   DROP FUNCTION public.generate_primary_key();
       public          postgres    false            �            1255    57346 d   sp_role_create_update_delete(text, bigint, character varying, text, boolean, bigint, bigint, bigint) 	   PROCEDURE     4  CREATE PROCEDURE public.sp_role_create_update_delete(IN p_action text, INOUT p_role_id bigint, IN p_role_name character varying, IN p_role_description text, IN p_is_active boolean, IN p_created_by bigint, IN p_updated_by bigint, IN p_deleted_by bigint, OUT p_message text)
    LANGUAGE plpgsql
    AS $$
BEGIN
    BEGIN
        CASE p_action
            WHEN 'CREATE' THEN
                -- Check if the role name already exists
                IF EXISTS (SELECT 1 FROM m_roles WHERE role_name = p_role_name) THEN
                    RAISE EXCEPTION 'Role name already exists';
                END IF;

                -- If role name is unique, proceed with insertion
                INSERT INTO m_roles(role_name, role_description, is_active, created_by, tenant_id)
                VALUES (p_role_name, p_role_description, p_is_active, p_created_by, p_created_by)
                RETURNING role_id INTO p_role_id;
                p_message := 'Role created successfully';

            WHEN 'UPDATE' THEN
                -- Update role details
                UPDATE m_roles
                SET role_name = COALESCE(p_role_name, role_name),
                    role_description = COALESCE(p_role_description, role_description),
                    is_active = COALESCE(p_is_active, is_active),
                    updated_on = CURRENT_TIMESTAMP,
                    updated_by = p_updated_by
                WHERE role_id = p_role_id;
                p_message := 'Role updated successfully';

            WHEN 'DELETE' THEN
                -- Soft delete role
                UPDATE m_roles
                SET is_active = false,
                    deleted_on = CURRENT_TIMESTAMP,
                    deleted_by = p_deleted_by
                WHERE role_id = p_role_id;
                p_message := 'Role deleted successfully';

            ELSE
                RAISE EXCEPTION 'Invalid action. Action must be one of: CREATE, UPDATE, DELETE';
        END CASE;
    EXCEPTION
        WHEN others THEN
            p_message := 'Error occurred: ' || SQLERRM;
            ROLLBACK;
    END;
END;
$$;
   DROP PROCEDURE public.sp_role_create_update_delete(IN p_action text, INOUT p_role_id bigint, IN p_role_name character varying, IN p_role_description text, IN p_is_active boolean, IN p_created_by bigint, IN p_updated_by bigint, IN p_deleted_by bigint, OUT p_message text);
       public          postgres    false            �            1255    57378 q   sp_role_create_update_delete(text, bigint, character varying, character varying, boolean, bigint, bigint, bigint) 	   PROCEDURE     k  CREATE PROCEDURE public.sp_role_create_update_delete(IN p_action text, INOUT p_role_id bigint, IN p_role_name character varying, IN p_role_description character varying, IN p_is_active boolean, IN p_created_by bigint, IN p_updated_by bigint, IN p_deleted_by bigint, OUT p_message text, OUT p_status_code integer)
    LANGUAGE plpgsql
    AS $$
BEGIN
    BEGIN
        CASE p_action
            WHEN 'CREATE' THEN
                -- Check if the role name already exists
                IF EXISTS (SELECT 1 FROM m_roles WHERE role_name = p_role_name) THEN
                    p_message := 'Role name already exists';
                    p_status_code := 409; -- Conflict
                ELSE
                    -- If role name is unique, proceed with insertion
                    INSERT INTO m_roles(role_name, role_description, is_active, created_by, tenant_id)
                    VALUES (p_role_name, p_role_description, p_is_active, p_created_by, p_created_by)
                    RETURNING role_id INTO p_role_id;
                    p_message := 'Role created successfully';
                    p_status_code := 201; -- Created
                END IF;

            WHEN 'UPDATE' THEN
                -- Check if the role exists
                IF NOT EXISTS (SELECT 1 FROM m_roles WHERE role_id = p_role_id) THEN
                    p_message := 'Role does not exist';
                    p_status_code := 404; -- Not Found
                ELSE
                    -- Update role details
                    UPDATE m_roles
                    SET role_name = COALESCE(p_role_name, role_name),
                        role_description = COALESCE(p_role_description, role_description),
                        is_active = COALESCE(p_is_active, is_active),
                        updated_on = CURRENT_TIMESTAMP,
                        updated_by = p_updated_by
                    WHERE role_id = p_role_id
                    RETURNING role_id INTO p_role_id;
                    p_message := 'Role updated successfully';
                    p_status_code := 200; -- OK
                END IF;

            WHEN 'DELETE' THEN
                -- Check if the role exists
                IF NOT EXISTS (SELECT 1 FROM m_roles WHERE role_id = p_role_id) THEN
                    p_message := 'Role does not exist';
                    p_status_code := 404; -- Not Found
                ELSE
                    -- Soft delete role
                    UPDATE m_roles
                    SET is_active = false,
                        deleted_on = CURRENT_TIMESTAMP,
                        deleted_by = p_deleted_by
                    WHERE role_id = p_role_id
                    RETURNING role_id INTO p_role_id;
                    p_message := 'Role deleted successfully';
                    p_status_code := 200; -- OK
                END IF;

            ELSE
                RAISE EXCEPTION 'Invalid action. Action must be one of: CREATE, UPDATE, DELETE';
        END CASE;
    EXCEPTION
        WHEN others THEN
            p_message := 'Error occurred: ' || SQLERRM;
            p_status_code := 500; -- Internal Server Error
            ROLLBACK;
    END;
END;
$$;
 8  DROP PROCEDURE public.sp_role_create_update_delete(IN p_action text, INOUT p_role_id bigint, IN p_role_name character varying, IN p_role_description character varying, IN p_is_active boolean, IN p_created_by bigint, IN p_updated_by bigint, IN p_deleted_by bigint, OUT p_message text, OUT p_status_code integer);
       public          postgres    false            �            1255    57377 �   sp_user_create_update_delete(text, bigint, character varying, character varying, character varying, boolean, bigint, bigint, bigint) 	   PROCEDURE     �  CREATE PROCEDURE public.sp_user_create_update_delete(IN p_action text, INOUT p_user_id bigint, IN p_username character varying, IN p_hashed_password character varying, IN p_email character varying, IN p_is_active boolean, IN p_created_by bigint, IN p_updated_by bigint, IN p_deleted_by bigint, OUT p_message text, OUT p_status_code integer)
    LANGUAGE plpgsql
    AS $$
BEGIN
    BEGIN
        CASE p_action
            WHEN 'CREATE' THEN
                -- Check if the email or username already exists
                IF EXISTS (SELECT 1 FROM t_users WHERE email = p_email) THEN
                    p_message := 'Email already exists';
                    p_status_code := 409; -- Conflict
                ELSE
                    -- If email and username are unique, proceed with insertion
                    INSERT INTO t_users(username, password, email, created_by, tenant_id)
                    VALUES (p_username, p_hashed_password, p_email, p_created_by, p_created_by)
                    RETURNING user_id INTO p_user_id;
                    p_message := 'User created successfully';
                    p_status_code := 201; -- Created
                END IF;

            WHEN 'UPDATE' THEN
                -- Check if the user exists
                IF NOT EXISTS (SELECT 1 FROM t_users WHERE user_id = p_user_id) THEN
                    p_message := 'User does not exist';
                    p_status_code := 404; -- Not Found
                ELSE
                    -- Update user details
                    UPDATE t_users
                    SET username = COALESCE(p_username, username),
                        password = COALESCE(p_hashed_password, password),
                        email = COALESCE(p_email, email),
                        is_active = COALESCE(p_is_active, is_active),
                        updated_on = CURRENT_TIMESTAMP,
                        updated_by = p_updated_by
                    WHERE user_id = p_user_id
                    RETURNING p_user_id INTO p_user_id;
                    p_message := 'User updated successfully';
                    p_status_code := 200; -- OK
                END IF;

            WHEN 'DELETE' THEN
                -- Check if the user exists
                IF NOT EXISTS (SELECT 1 FROM t_users WHERE user_id = p_user_id) THEN
                    p_message := 'User does not exist';
                    p_status_code := 404; -- Not Found
                ELSE
                    -- Soft delete user
                    UPDATE t_users
                    SET is_active = false,
                        deleted_on = CURRENT_TIMESTAMP,
                        deleted_by = p_deleted_by
                    WHERE user_id = p_user_id
                    RETURNING p_user_id INTO p_user_id;
                    p_message := 'User deleted successfully';
                    p_status_code := 200; -- OK
                END IF;

            ELSE
                RAISE EXCEPTION 'Invalid action. Action must be one of: CREATE, UPDATE, DELETE';
        END CASE;
    EXCEPTION
        WHEN others THEN
            p_message := 'Error occurred: ' || SQLERRM;
            p_status_code := 500; -- Internal Server Error
            ROLLBACK;
    END;
END;
$$;
 T  DROP PROCEDURE public.sp_user_create_update_delete(IN p_action text, INOUT p_user_id bigint, IN p_username character varying, IN p_hashed_password character varying, IN p_email character varying, IN p_is_active boolean, IN p_created_by bigint, IN p_updated_by bigint, IN p_deleted_by bigint, OUT p_message text, OUT p_status_code integer);
       public          postgres    false            �            1259    49327    m_permissions    TABLE     �  CREATE TABLE public.m_permissions (
    permission_id bigint NOT NULL,
    permission_name character varying(255) NOT NULL,
    permission_description text,
    is_active boolean DEFAULT true,
    created_on timestamp with time zone DEFAULT CURRENT_TIMESTAMP,
    created_by bigint,
    updated_on timestamp with time zone,
    updated_by bigint,
    deleted_on timestamp with time zone,
    deleted_by bigint,
    tenant_id bigint
);
 !   DROP TABLE public.m_permissions;
       public         heap    postgres    false            �            1259    49326    m_permissions_permission_id_seq    SEQUENCE     �   CREATE SEQUENCE public.m_permissions_permission_id_seq
    START WITH 1
    INCREMENT BY 1
    NO MINVALUE
    NO MAXVALUE
    CACHE 1;
 6   DROP SEQUENCE public.m_permissions_permission_id_seq;
       public          postgres    false    219            L           0    0    m_permissions_permission_id_seq    SEQUENCE OWNED BY     c   ALTER SEQUENCE public.m_permissions_permission_id_seq OWNED BY public.m_permissions.permission_id;
          public          postgres    false    218            �            1259    49314    m_roles    TABLE     �  CREATE TABLE public.m_roles (
    role_id bigint DEFAULT public.generate_primary_key() NOT NULL,
    role_name character varying(255) NOT NULL,
    role_description text,
    is_active boolean DEFAULT true,
    created_on timestamp with time zone DEFAULT CURRENT_TIMESTAMP,
    created_by bigint,
    updated_on timestamp with time zone,
    updated_by bigint,
    deleted_on timestamp with time zone,
    deleted_by bigint,
    tenant_id bigint
);
    DROP TABLE public.m_roles;
       public         heap    postgres    false    225            �            1259    49313    m_roles_role_id_seq    SEQUENCE     |   CREATE SEQUENCE public.m_roles_role_id_seq
    START WITH 1
    INCREMENT BY 1
    NO MINVALUE
    NO MAXVALUE
    CACHE 1;
 *   DROP SEQUENCE public.m_roles_role_id_seq;
       public          postgres    false    217            M           0    0    m_roles_role_id_seq    SEQUENCE OWNED BY     K   ALTER SEQUENCE public.m_roles_role_id_seq OWNED BY public.m_roles.role_id;
          public          postgres    false    216            �            1259    49415    pk_sequence    SEQUENCE     {   CREATE SEQUENCE public.pk_sequence
    START WITH 1
    INCREMENT BY 1
    NO MINVALUE
    MAXVALUE 999999999
    CACHE 1;
 "   DROP SEQUENCE public.pk_sequence;
       public          postgres    false            �            1259    49417    row_sequence    SEQUENCE     |   CREATE SEQUENCE public.row_sequence
    START WITH 1
    INCREMENT BY 1
    NO MINVALUE
    MAXVALUE 999999999
    CACHE 1;
 #   DROP SEQUENCE public.row_sequence;
       public          postgres    false            �            1259    49356    t_role_permissions    TABLE       CREATE TABLE public.t_role_permissions (
    role_id bigint NOT NULL,
    permission_id bigint NOT NULL,
    is_active boolean DEFAULT true,
    created_on timestamp with time zone DEFAULT CURRENT_TIMESTAMP,
    created_by bigint,
    updated_on timestamp with time zone,
    updated_by bigint,
    deleted_on timestamp with time zone,
    deleted_by bigint,
    tenant_id bigint
);
 &   DROP TABLE public.t_role_permissions;
       public         heap    postgres    false            �            1259    49339    t_user_roles    TABLE     s  CREATE TABLE public.t_user_roles (
    user_id bigint NOT NULL,
    role_id bigint NOT NULL,
    is_active boolean DEFAULT true,
    created_on timestamp with time zone DEFAULT CURRENT_TIMESTAMP,
    created_by bigint,
    updated_on timestamp with time zone,
    updated_by bigint,
    deleted_on timestamp with time zone,
    deleted_by bigint,
    tenant_id bigint
);
     DROP TABLE public.t_user_roles;
       public         heap    postgres    false            �            1259    49373    t_user_roles_permissions    TABLE     �  CREATE TABLE public.t_user_roles_permissions (
    user_id bigint NOT NULL,
    role_id bigint NOT NULL,
    permission_id bigint NOT NULL,
    is_active boolean DEFAULT true,
    created_on timestamp with time zone DEFAULT CURRENT_TIMESTAMP,
    created_by bigint,
    updated_on timestamp with time zone,
    updated_by bigint,
    deleted_on timestamp with time zone,
    deleted_by bigint,
    tenant_id bigint
);
 ,   DROP TABLE public.t_user_roles_permissions;
       public         heap    postgres    false            �            1259    49299    t_users    TABLE     �  CREATE TABLE public.t_users (
    user_id bigint DEFAULT public.generate_primary_key() NOT NULL,
    username character varying(255) NOT NULL,
    password character varying NOT NULL,
    email character varying(255) NOT NULL,
    is_active boolean DEFAULT true,
    created_on timestamp with time zone DEFAULT CURRENT_TIMESTAMP,
    created_by bigint,
    updated_on timestamp with time zone,
    updated_by bigint,
    deleted_on timestamp with time zone,
    deleted_by bigint,
    tenant_id bigint
);
    DROP TABLE public.t_users;
       public         heap    postgres    false    225            �            1259    49298    t_users_user_id_seq    SEQUENCE     |   CREATE SEQUENCE public.t_users_user_id_seq
    START WITH 1
    INCREMENT BY 1
    NO MINVALUE
    NO MAXVALUE
    CACHE 1;
 *   DROP SEQUENCE public.t_users_user_id_seq;
       public          postgres    false    215            N           0    0    t_users_user_id_seq    SEQUENCE OWNED BY     K   ALTER SEQUENCE public.t_users_user_id_seq OWNED BY public.t_users.user_id;
          public          postgres    false    214            �           2604    49330    m_permissions permission_id    DEFAULT     �   ALTER TABLE ONLY public.m_permissions ALTER COLUMN permission_id SET DEFAULT nextval('public.m_permissions_permission_id_seq'::regclass);
 J   ALTER TABLE public.m_permissions ALTER COLUMN permission_id DROP DEFAULT;
       public          postgres    false    218    219    219            @          0    49327    m_permissions 
   TABLE DATA           �   COPY public.m_permissions (permission_id, permission_name, permission_description, is_active, created_on, created_by, updated_on, updated_by, deleted_on, deleted_by, tenant_id) FROM stdin;
    public          postgres    false    219   ��       >          0    49314    m_roles 
   TABLE DATA           �   COPY public.m_roles (role_id, role_name, role_description, is_active, created_on, created_by, updated_on, updated_by, deleted_on, deleted_by, tenant_id) FROM stdin;
    public          postgres    false    217   ��       B          0    49356    t_role_permissions 
   TABLE DATA           �   COPY public.t_role_permissions (role_id, permission_id, is_active, created_on, created_by, updated_on, updated_by, deleted_on, deleted_by, tenant_id) FROM stdin;
    public          postgres    false    221   6�       A          0    49339    t_user_roles 
   TABLE DATA           �   COPY public.t_user_roles (user_id, role_id, is_active, created_on, created_by, updated_on, updated_by, deleted_on, deleted_by, tenant_id) FROM stdin;
    public          postgres    false    220   S�       C          0    49373    t_user_roles_permissions 
   TABLE DATA           �   COPY public.t_user_roles_permissions (user_id, role_id, permission_id, is_active, created_on, created_by, updated_on, updated_by, deleted_on, deleted_by, tenant_id) FROM stdin;
    public          postgres    false    222   p�       <          0    49299    t_users 
   TABLE DATA           �   COPY public.t_users (user_id, username, password, email, is_active, created_on, created_by, updated_on, updated_by, deleted_on, deleted_by, tenant_id) FROM stdin;
    public          postgres    false    215   ��       O           0    0    m_permissions_permission_id_seq    SEQUENCE SET     N   SELECT pg_catalog.setval('public.m_permissions_permission_id_seq', 1, false);
          public          postgres    false    218            P           0    0    m_roles_role_id_seq    SEQUENCE SET     A   SELECT pg_catalog.setval('public.m_roles_role_id_seq', 1, true);
          public          postgres    false    216            Q           0    0    pk_sequence    SEQUENCE SET     9   SELECT pg_catalog.setval('public.pk_sequence', 6, true);
          public          postgres    false    223            R           0    0    row_sequence    SEQUENCE SET     ;   SELECT pg_catalog.setval('public.row_sequence', 30, true);
          public          postgres    false    224            S           0    0    t_users_user_id_seq    SEQUENCE SET     B   SELECT pg_catalog.setval('public.t_users_user_id_seq', 10, true);
          public          postgres    false    214            �           2606    49338 /   m_permissions m_permissions_permission_name_key 
   CONSTRAINT     u   ALTER TABLE ONLY public.m_permissions
    ADD CONSTRAINT m_permissions_permission_name_key UNIQUE (permission_name);
 Y   ALTER TABLE ONLY public.m_permissions DROP CONSTRAINT m_permissions_permission_name_key;
       public            postgres    false    219            �           2606    49336     m_permissions m_permissions_pkey 
   CONSTRAINT     i   ALTER TABLE ONLY public.m_permissions
    ADD CONSTRAINT m_permissions_pkey PRIMARY KEY (permission_id);
 J   ALTER TABLE ONLY public.m_permissions DROP CONSTRAINT m_permissions_pkey;
       public            postgres    false    219            �           2606    49323    m_roles m_roles_pkey 
   CONSTRAINT     W   ALTER TABLE ONLY public.m_roles
    ADD CONSTRAINT m_roles_pkey PRIMARY KEY (role_id);
 >   ALTER TABLE ONLY public.m_roles DROP CONSTRAINT m_roles_pkey;
       public            postgres    false    217            �           2606    49325    m_roles m_roles_role_name_key 
   CONSTRAINT     ]   ALTER TABLE ONLY public.m_roles
    ADD CONSTRAINT m_roles_role_name_key UNIQUE (role_name);
 G   ALTER TABLE ONLY public.m_roles DROP CONSTRAINT m_roles_role_name_key;
       public            postgres    false    217            �           2606    49362 *   t_role_permissions t_role_permissions_pkey 
   CONSTRAINT     |   ALTER TABLE ONLY public.t_role_permissions
    ADD CONSTRAINT t_role_permissions_pkey PRIMARY KEY (role_id, permission_id);
 T   ALTER TABLE ONLY public.t_role_permissions DROP CONSTRAINT t_role_permissions_pkey;
       public            postgres    false    221    221            �           2606    49345    t_user_roles t_user_roles_pkey 
   CONSTRAINT     j   ALTER TABLE ONLY public.t_user_roles
    ADD CONSTRAINT t_user_roles_pkey PRIMARY KEY (user_id, role_id);
 H   ALTER TABLE ONLY public.t_user_roles DROP CONSTRAINT t_user_roles_pkey;
       public            postgres    false    220    220            �           2606    49312    t_users t_users_email_key 
   CONSTRAINT     U   ALTER TABLE ONLY public.t_users
    ADD CONSTRAINT t_users_email_key UNIQUE (email);
 C   ALTER TABLE ONLY public.t_users DROP CONSTRAINT t_users_email_key;
       public            postgres    false    215            �           2606    49308    t_users t_users_pkey 
   CONSTRAINT     W   ALTER TABLE ONLY public.t_users
    ADD CONSTRAINT t_users_pkey PRIMARY KEY (user_id);
 >   ALTER TABLE ONLY public.t_users DROP CONSTRAINT t_users_pkey;
       public            postgres    false    215            �           2606    49310    t_users t_users_username_key 
   CONSTRAINT     [   ALTER TABLE ONLY public.t_users
    ADD CONSTRAINT t_users_username_key UNIQUE (username);
 F   ALTER TABLE ONLY public.t_users DROP CONSTRAINT t_users_username_key;
       public            postgres    false    215            �           2606    49368 8   t_role_permissions t_role_permissions_permission_id_fkey    FK CONSTRAINT     �   ALTER TABLE ONLY public.t_role_permissions
    ADD CONSTRAINT t_role_permissions_permission_id_fkey FOREIGN KEY (permission_id) REFERENCES public.m_permissions(permission_id);
 b   ALTER TABLE ONLY public.t_role_permissions DROP CONSTRAINT t_role_permissions_permission_id_fkey;
       public          postgres    false    3233    221    219            �           2606    49363 2   t_role_permissions t_role_permissions_role_id_fkey    FK CONSTRAINT     �   ALTER TABLE ONLY public.t_role_permissions
    ADD CONSTRAINT t_role_permissions_role_id_fkey FOREIGN KEY (role_id) REFERENCES public.m_roles(role_id);
 \   ALTER TABLE ONLY public.t_role_permissions DROP CONSTRAINT t_role_permissions_role_id_fkey;
       public          postgres    false    221    217    3227            �           2606    49388 D   t_user_roles_permissions t_user_roles_permissions_permission_id_fkey    FK CONSTRAINT     �   ALTER TABLE ONLY public.t_user_roles_permissions
    ADD CONSTRAINT t_user_roles_permissions_permission_id_fkey FOREIGN KEY (permission_id) REFERENCES public.m_permissions(permission_id);
 n   ALTER TABLE ONLY public.t_user_roles_permissions DROP CONSTRAINT t_user_roles_permissions_permission_id_fkey;
       public          postgres    false    3233    219    222            �           2606    49383 >   t_user_roles_permissions t_user_roles_permissions_role_id_fkey    FK CONSTRAINT     �   ALTER TABLE ONLY public.t_user_roles_permissions
    ADD CONSTRAINT t_user_roles_permissions_role_id_fkey FOREIGN KEY (role_id) REFERENCES public.m_roles(role_id);
 h   ALTER TABLE ONLY public.t_user_roles_permissions DROP CONSTRAINT t_user_roles_permissions_role_id_fkey;
       public          postgres    false    3227    222    217            �           2606    49378 >   t_user_roles_permissions t_user_roles_permissions_user_id_fkey    FK CONSTRAINT     �   ALTER TABLE ONLY public.t_user_roles_permissions
    ADD CONSTRAINT t_user_roles_permissions_user_id_fkey FOREIGN KEY (user_id) REFERENCES public.t_users(user_id);
 h   ALTER TABLE ONLY public.t_user_roles_permissions DROP CONSTRAINT t_user_roles_permissions_user_id_fkey;
       public          postgres    false    222    215    3223            �           2606    49351 &   t_user_roles t_user_roles_role_id_fkey    FK CONSTRAINT     �   ALTER TABLE ONLY public.t_user_roles
    ADD CONSTRAINT t_user_roles_role_id_fkey FOREIGN KEY (role_id) REFERENCES public.m_roles(role_id);
 P   ALTER TABLE ONLY public.t_user_roles DROP CONSTRAINT t_user_roles_role_id_fkey;
       public          postgres    false    220    217    3227            �           2606    49346 &   t_user_roles t_user_roles_user_id_fkey    FK CONSTRAINT     �   ALTER TABLE ONLY public.t_user_roles
    ADD CONSTRAINT t_user_roles_user_id_fkey FOREIGN KEY (user_id) REFERENCES public.t_users(user_id);
 P   ALTER TABLE ONLY public.t_user_roles DROP CONSTRAINT t_user_roles_user_id_fkey;
       public          postgres    false    220    3223    215            @      x������ � �      >   �   x�u�;�@D��S�GXc{�w}N�	�R����P��ϔo�Éߗ��4_�%	��a�;�Ys`�H�B������J����@��*=,�N��4o.�( ���1�s�����l�����/�      B      x������ � �      A      x������ � �      C      x������ � �      <   �   x�u��
�0E��W���IRmV�J�T,� n�Z�X����ƅO����2�!Kq,|!6I~��&���b����cJ�s1L'�κN��|VW�Uv-�>�Oo[��^��8
��@�,����]Mh�u��2}�� 
��^���~��2R��a����XgI1b�F��U�/r���w�A�     