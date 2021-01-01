IF OBJECT_ID(N'[__EFMigrationsHistory]') IS NULL
BEGIN
    CREATE TABLE [__EFMigrationsHistory] (
        [MigrationId] nvarchar(150) NOT NULL,
        [ProductVersion] nvarchar(32) NOT NULL,
        CONSTRAINT [PK___EFMigrationsHistory] PRIMARY KEY ([MigrationId])
    );
END;

GO

IF NOT EXISTS(SELECT * FROM [__EFMigrationsHistory] WHERE [MigrationId] = N'00000000000000_CreateIdentitySchema')
BEGIN
    CREATE TABLE [AspNetRoles] (
        [Id] nvarchar(450) NOT NULL,
        [Name] nvarchar(256) NULL,
        [NormalizedName] nvarchar(256) NULL,
        [ConcurrencyStamp] nvarchar(max) NULL,
        CONSTRAINT [PK_AspNetRoles] PRIMARY KEY ([Id])
    );
END;

GO

IF NOT EXISTS(SELECT * FROM [__EFMigrationsHistory] WHERE [MigrationId] = N'00000000000000_CreateIdentitySchema')
BEGIN
    CREATE TABLE [AspNetUsers] (
        [Id] nvarchar(450) NOT NULL,
        [UserName] nvarchar(256) NULL,
        [NormalizedUserName] nvarchar(256) NULL,
        [Email] nvarchar(256) NULL,
        [NormalizedEmail] nvarchar(256) NULL,
        [EmailConfirmed] bit NOT NULL,
        [PasswordHash] nvarchar(max) NULL,
        [SecurityStamp] nvarchar(max) NULL,
        [ConcurrencyStamp] nvarchar(max) NULL,
        [PhoneNumber] nvarchar(max) NULL,
        [PhoneNumberConfirmed] bit NOT NULL,
        [TwoFactorEnabled] bit NOT NULL,
        [LockoutEnd] datetimeoffset NULL,
        [LockoutEnabled] bit NOT NULL,
        [AccessFailedCount] int NOT NULL,
        CONSTRAINT [PK_AspNetUsers] PRIMARY KEY ([Id])
    );
END;

GO

IF NOT EXISTS(SELECT * FROM [__EFMigrationsHistory] WHERE [MigrationId] = N'00000000000000_CreateIdentitySchema')
BEGIN
    CREATE TABLE [AspNetRoleClaims] (
        [Id] int NOT NULL IDENTITY,
        [RoleId] nvarchar(450) NOT NULL,
        [ClaimType] nvarchar(max) NULL,
        [ClaimValue] nvarchar(max) NULL,
        CONSTRAINT [PK_AspNetRoleClaims] PRIMARY KEY ([Id]),
        CONSTRAINT [FK_AspNetRoleClaims_AspNetRoles_RoleId] FOREIGN KEY ([RoleId]) REFERENCES [AspNetRoles] ([Id]) ON DELETE CASCADE
    );
END;

GO

IF NOT EXISTS(SELECT * FROM [__EFMigrationsHistory] WHERE [MigrationId] = N'00000000000000_CreateIdentitySchema')
BEGIN
    CREATE TABLE [AspNetUserClaims] (
        [Id] int NOT NULL IDENTITY,
        [UserId] nvarchar(450) NOT NULL,
        [ClaimType] nvarchar(max) NULL,
        [ClaimValue] nvarchar(max) NULL,
        CONSTRAINT [PK_AspNetUserClaims] PRIMARY KEY ([Id]),
        CONSTRAINT [FK_AspNetUserClaims_AspNetUsers_UserId] FOREIGN KEY ([UserId]) REFERENCES [AspNetUsers] ([Id]) ON DELETE CASCADE
    );
END;

GO

IF NOT EXISTS(SELECT * FROM [__EFMigrationsHistory] WHERE [MigrationId] = N'00000000000000_CreateIdentitySchema')
BEGIN
    CREATE TABLE [AspNetUserLogins] (
        [LoginProvider] nvarchar(128) NOT NULL,
        [ProviderKey] nvarchar(128) NOT NULL,
        [ProviderDisplayName] nvarchar(max) NULL,
        [UserId] nvarchar(450) NOT NULL,
        CONSTRAINT [PK_AspNetUserLogins] PRIMARY KEY ([LoginProvider], [ProviderKey]),
        CONSTRAINT [FK_AspNetUserLogins_AspNetUsers_UserId] FOREIGN KEY ([UserId]) REFERENCES [AspNetUsers] ([Id]) ON DELETE CASCADE
    );
END;

GO

IF NOT EXISTS(SELECT * FROM [__EFMigrationsHistory] WHERE [MigrationId] = N'00000000000000_CreateIdentitySchema')
BEGIN
    CREATE TABLE [AspNetUserRoles] (
        [UserId] nvarchar(450) NOT NULL,
        [RoleId] nvarchar(450) NOT NULL,
        CONSTRAINT [PK_AspNetUserRoles] PRIMARY KEY ([UserId], [RoleId]),
        CONSTRAINT [FK_AspNetUserRoles_AspNetRoles_RoleId] FOREIGN KEY ([RoleId]) REFERENCES [AspNetRoles] ([Id]) ON DELETE CASCADE,
        CONSTRAINT [FK_AspNetUserRoles_AspNetUsers_UserId] FOREIGN KEY ([UserId]) REFERENCES [AspNetUsers] ([Id]) ON DELETE CASCADE
    );
END;

GO

IF NOT EXISTS(SELECT * FROM [__EFMigrationsHistory] WHERE [MigrationId] = N'00000000000000_CreateIdentitySchema')
BEGIN
    CREATE TABLE [AspNetUserTokens] (
        [UserId] nvarchar(450) NOT NULL,
        [LoginProvider] nvarchar(128) NOT NULL,
        [Name] nvarchar(128) NOT NULL,
        [Value] nvarchar(max) NULL,
        CONSTRAINT [PK_AspNetUserTokens] PRIMARY KEY ([UserId], [LoginProvider], [Name]),
        CONSTRAINT [FK_AspNetUserTokens_AspNetUsers_UserId] FOREIGN KEY ([UserId]) REFERENCES [AspNetUsers] ([Id]) ON DELETE CASCADE
    );
END;

GO

IF NOT EXISTS(SELECT * FROM [__EFMigrationsHistory] WHERE [MigrationId] = N'00000000000000_CreateIdentitySchema')
BEGIN
    CREATE INDEX [IX_AspNetRoleClaims_RoleId] ON [AspNetRoleClaims] ([RoleId]);
END;

GO

IF NOT EXISTS(SELECT * FROM [__EFMigrationsHistory] WHERE [MigrationId] = N'00000000000000_CreateIdentitySchema')
BEGIN
    CREATE UNIQUE INDEX [RoleNameIndex] ON [AspNetRoles] ([NormalizedName]) WHERE [NormalizedName] IS NOT NULL;
END;

GO

IF NOT EXISTS(SELECT * FROM [__EFMigrationsHistory] WHERE [MigrationId] = N'00000000000000_CreateIdentitySchema')
BEGIN
    CREATE INDEX [IX_AspNetUserClaims_UserId] ON [AspNetUserClaims] ([UserId]);
END;

GO

IF NOT EXISTS(SELECT * FROM [__EFMigrationsHistory] WHERE [MigrationId] = N'00000000000000_CreateIdentitySchema')
BEGIN
    CREATE INDEX [IX_AspNetUserLogins_UserId] ON [AspNetUserLogins] ([UserId]);
END;

GO

IF NOT EXISTS(SELECT * FROM [__EFMigrationsHistory] WHERE [MigrationId] = N'00000000000000_CreateIdentitySchema')
BEGIN
    CREATE INDEX [IX_AspNetUserRoles_RoleId] ON [AspNetUserRoles] ([RoleId]);
END;

GO

IF NOT EXISTS(SELECT * FROM [__EFMigrationsHistory] WHERE [MigrationId] = N'00000000000000_CreateIdentitySchema')
BEGIN
    CREATE INDEX [EmailIndex] ON [AspNetUsers] ([NormalizedEmail]);
END;

GO

IF NOT EXISTS(SELECT * FROM [__EFMigrationsHistory] WHERE [MigrationId] = N'00000000000000_CreateIdentitySchema')
BEGIN
    CREATE UNIQUE INDEX [UserNameIndex] ON [AspNetUsers] ([NormalizedUserName]) WHERE [NormalizedUserName] IS NOT NULL;
END;

GO

IF NOT EXISTS(SELECT * FROM [__EFMigrationsHistory] WHERE [MigrationId] = N'00000000000000_CreateIdentitySchema')
BEGIN
    INSERT INTO [__EFMigrationsHistory] ([MigrationId], [ProductVersion])
    VALUES (N'00000000000000_CreateIdentitySchema', N'3.1.8');
END;

GO

IF NOT EXISTS(SELECT * FROM [__EFMigrationsHistory] WHERE [MigrationId] = N'20200925143313_AddCategoryToDatabase')
BEGIN
    CREATE TABLE [Category] (
        [Id] int NOT NULL IDENTITY,
        [Name] nvarchar(max) NOT NULL,
        CONSTRAINT [PK_Category] PRIMARY KEY ([Id])
    );
END;

GO

IF NOT EXISTS(SELECT * FROM [__EFMigrationsHistory] WHERE [MigrationId] = N'20200925143313_AddCategoryToDatabase')
BEGIN
    INSERT INTO [__EFMigrationsHistory] ([MigrationId], [ProductVersion])
    VALUES (N'20200925143313_AddCategoryToDatabase', N'3.1.8');
END;

GO

IF NOT EXISTS(SELECT * FROM [__EFMigrationsHistory] WHERE [MigrationId] = N'20200926161206_addSubCategoryToDatabase')
BEGIN
    CREATE TABLE [SubCategory] (
        [Id] int NOT NULL IDENTITY,
        [Name] nvarchar(max) NOT NULL,
        [CategoryId] int NOT NULL,
        CONSTRAINT [PK_SubCategory] PRIMARY KEY ([Id]),
        CONSTRAINT [FK_SubCategory_Category_CategoryId] FOREIGN KEY ([CategoryId]) REFERENCES [Category] ([Id]) ON DELETE CASCADE
    );
END;

GO

IF NOT EXISTS(SELECT * FROM [__EFMigrationsHistory] WHERE [MigrationId] = N'20200926161206_addSubCategoryToDatabase')
BEGIN
    CREATE INDEX [IX_SubCategory_CategoryId] ON [SubCategory] ([CategoryId]);
END;

GO

IF NOT EXISTS(SELECT * FROM [__EFMigrationsHistory] WHERE [MigrationId] = N'20200926161206_addSubCategoryToDatabase')
BEGIN
    INSERT INTO [__EFMigrationsHistory] ([MigrationId], [ProductVersion])
    VALUES (N'20200926161206_addSubCategoryToDatabase', N'3.1.8');
END;

GO

IF NOT EXISTS(SELECT * FROM [__EFMigrationsHistory] WHERE [MigrationId] = N'20200930174911_AddMenuItemToDatabase')
BEGIN
    CREATE TABLE [MenuItem] (
        [Id] int NOT NULL IDENTITY,
        [Name] nvarchar(max) NOT NULL,
        [Description] nvarchar(max) NULL,
        [Spicyness] nvarchar(max) NULL,
        [Image] nvarchar(max) NULL,
        [CategoryId] int NOT NULL,
        [SubCategoryId] int NOT NULL,
        [Price] float NOT NULL,
        CONSTRAINT [PK_MenuItem] PRIMARY KEY ([Id]),
        CONSTRAINT [FK_MenuItem_Category_CategoryId] FOREIGN KEY ([CategoryId]) REFERENCES [Category] ([Id]),
        CONSTRAINT [FK_MenuItem_SubCategory_SubCategoryId] FOREIGN KEY ([SubCategoryId]) REFERENCES [SubCategory] ([Id])
    );
END;

GO

IF NOT EXISTS(SELECT * FROM [__EFMigrationsHistory] WHERE [MigrationId] = N'20200930174911_AddMenuItemToDatabase')
BEGIN
    CREATE INDEX [IX_MenuItem_CategoryId] ON [MenuItem] ([CategoryId]);
END;

GO

IF NOT EXISTS(SELECT * FROM [__EFMigrationsHistory] WHERE [MigrationId] = N'20200930174911_AddMenuItemToDatabase')
BEGIN
    CREATE INDEX [IX_MenuItem_SubCategoryId] ON [MenuItem] ([SubCategoryId]);
END;

GO

IF NOT EXISTS(SELECT * FROM [__EFMigrationsHistory] WHERE [MigrationId] = N'20200930174911_AddMenuItemToDatabase')
BEGIN
    INSERT INTO [__EFMigrationsHistory] ([MigrationId], [ProductVersion])
    VALUES (N'20200930174911_AddMenuItemToDatabase', N'3.1.8');
END;

GO

IF NOT EXISTS(SELECT * FROM [__EFMigrationsHistory] WHERE [MigrationId] = N'20201002162757_AddCouponToDatabase')
BEGIN
    CREATE TABLE [Coupon] (
        [Id] int NOT NULL IDENTITY,
        [Name] nvarchar(max) NOT NULL,
        [CouponType] nvarchar(max) NOT NULL,
        [Discount] float NOT NULL,
        [MinumumAmount] float NOT NULL,
        [Picture] varbinary(max) NULL,
        [IsActive] bit NOT NULL,
        CONSTRAINT [PK_Coupon] PRIMARY KEY ([Id])
    );
END;

GO

IF NOT EXISTS(SELECT * FROM [__EFMigrationsHistory] WHERE [MigrationId] = N'20201002162757_AddCouponToDatabase')
BEGIN
    INSERT INTO [__EFMigrationsHistory] ([MigrationId], [ProductVersion])
    VALUES (N'20201002162757_AddCouponToDatabase', N'3.1.8');
END;

GO

IF NOT EXISTS(SELECT * FROM [__EFMigrationsHistory] WHERE [MigrationId] = N'20201008144731_AddMoreFieldsToIdentityUser')
BEGIN
    ALTER TABLE [AspNetUsers] ADD [Discriminator] nvarchar(max) NOT NULL DEFAULT N'';
END;

GO

IF NOT EXISTS(SELECT * FROM [__EFMigrationsHistory] WHERE [MigrationId] = N'20201008144731_AddMoreFieldsToIdentityUser')
BEGIN
    ALTER TABLE [AspNetUsers] ADD [City] nvarchar(max) NULL;
END;

GO

IF NOT EXISTS(SELECT * FROM [__EFMigrationsHistory] WHERE [MigrationId] = N'20201008144731_AddMoreFieldsToIdentityUser')
BEGIN
    ALTER TABLE [AspNetUsers] ADD [Name] nvarchar(max) NULL;
END;

GO

IF NOT EXISTS(SELECT * FROM [__EFMigrationsHistory] WHERE [MigrationId] = N'20201008144731_AddMoreFieldsToIdentityUser')
BEGIN
    ALTER TABLE [AspNetUsers] ADD [PostalCode] nvarchar(max) NULL;
END;

GO

IF NOT EXISTS(SELECT * FROM [__EFMigrationsHistory] WHERE [MigrationId] = N'20201008144731_AddMoreFieldsToIdentityUser')
BEGIN
    ALTER TABLE [AspNetUsers] ADD [State] nvarchar(max) NULL;
END;

GO

IF NOT EXISTS(SELECT * FROM [__EFMigrationsHistory] WHERE [MigrationId] = N'20201008144731_AddMoreFieldsToIdentityUser')
BEGIN
    ALTER TABLE [AspNetUsers] ADD [StreetAddress] nvarchar(max) NULL;
END;

GO

IF NOT EXISTS(SELECT * FROM [__EFMigrationsHistory] WHERE [MigrationId] = N'20201008144731_AddMoreFieldsToIdentityUser')
BEGIN
    INSERT INTO [__EFMigrationsHistory] ([MigrationId], [ProductVersion])
    VALUES (N'20201008144731_AddMoreFieldsToIdentityUser', N'3.1.8');
END;

GO

IF NOT EXISTS(SELECT * FROM [__EFMigrationsHistory] WHERE [MigrationId] = N'20201013174246_AddShoppingCartToDb')
BEGIN
    DECLARE @var0 sysname;
    SELECT @var0 = [d].[name]
    FROM [sys].[default_constraints] [d]
    INNER JOIN [sys].[columns] [c] ON [d].[parent_column_id] = [c].[column_id] AND [d].[parent_object_id] = [c].[object_id]
    WHERE ([d].[parent_object_id] = OBJECT_ID(N'[AspNetUserTokens]') AND [c].[name] = N'Name');
    IF @var0 IS NOT NULL EXEC(N'ALTER TABLE [AspNetUserTokens] DROP CONSTRAINT [' + @var0 + '];');
    ALTER TABLE [AspNetUserTokens] ALTER COLUMN [Name] nvarchar(450) NOT NULL;
END;

GO

IF NOT EXISTS(SELECT * FROM [__EFMigrationsHistory] WHERE [MigrationId] = N'20201013174246_AddShoppingCartToDb')
BEGIN
    DECLARE @var1 sysname;
    SELECT @var1 = [d].[name]
    FROM [sys].[default_constraints] [d]
    INNER JOIN [sys].[columns] [c] ON [d].[parent_column_id] = [c].[column_id] AND [d].[parent_object_id] = [c].[object_id]
    WHERE ([d].[parent_object_id] = OBJECT_ID(N'[AspNetUserTokens]') AND [c].[name] = N'LoginProvider');
    IF @var1 IS NOT NULL EXEC(N'ALTER TABLE [AspNetUserTokens] DROP CONSTRAINT [' + @var1 + '];');
    ALTER TABLE [AspNetUserTokens] ALTER COLUMN [LoginProvider] nvarchar(450) NOT NULL;
END;

GO

IF NOT EXISTS(SELECT * FROM [__EFMigrationsHistory] WHERE [MigrationId] = N'20201013174246_AddShoppingCartToDb')
BEGIN
    DECLARE @var2 sysname;
    SELECT @var2 = [d].[name]
    FROM [sys].[default_constraints] [d]
    INNER JOIN [sys].[columns] [c] ON [d].[parent_column_id] = [c].[column_id] AND [d].[parent_object_id] = [c].[object_id]
    WHERE ([d].[parent_object_id] = OBJECT_ID(N'[AspNetUserLogins]') AND [c].[name] = N'ProviderKey');
    IF @var2 IS NOT NULL EXEC(N'ALTER TABLE [AspNetUserLogins] DROP CONSTRAINT [' + @var2 + '];');
    ALTER TABLE [AspNetUserLogins] ALTER COLUMN [ProviderKey] nvarchar(450) NOT NULL;
END;

GO

IF NOT EXISTS(SELECT * FROM [__EFMigrationsHistory] WHERE [MigrationId] = N'20201013174246_AddShoppingCartToDb')
BEGIN
    DECLARE @var3 sysname;
    SELECT @var3 = [d].[name]
    FROM [sys].[default_constraints] [d]
    INNER JOIN [sys].[columns] [c] ON [d].[parent_column_id] = [c].[column_id] AND [d].[parent_object_id] = [c].[object_id]
    WHERE ([d].[parent_object_id] = OBJECT_ID(N'[AspNetUserLogins]') AND [c].[name] = N'LoginProvider');
    IF @var3 IS NOT NULL EXEC(N'ALTER TABLE [AspNetUserLogins] DROP CONSTRAINT [' + @var3 + '];');
    ALTER TABLE [AspNetUserLogins] ALTER COLUMN [LoginProvider] nvarchar(450) NOT NULL;
END;

GO

IF NOT EXISTS(SELECT * FROM [__EFMigrationsHistory] WHERE [MigrationId] = N'20201013174246_AddShoppingCartToDb')
BEGIN
    CREATE TABLE [ShoppingCart] (
        [Id] int NOT NULL IDENTITY,
        [ApplicationUserId] nvarchar(max) NULL,
        [MenuItemId] int NOT NULL,
        [Count] int NOT NULL,
        CONSTRAINT [PK_ShoppingCart] PRIMARY KEY ([Id])
    );
END;

GO

IF NOT EXISTS(SELECT * FROM [__EFMigrationsHistory] WHERE [MigrationId] = N'20201013174246_AddShoppingCartToDb')
BEGIN
    INSERT INTO [__EFMigrationsHistory] ([MigrationId], [ProductVersion])
    VALUES (N'20201013174246_AddShoppingCartToDb', N'3.1.8');
END;

GO

IF NOT EXISTS(SELECT * FROM [__EFMigrationsHistory] WHERE [MigrationId] = N'20201024014826_AddOrderHeaderAndDetailsToDB')
BEGIN
    CREATE TABLE [OrderHeader] (
        [Id] int NOT NULL IDENTITY,
        [UserId] nvarchar(450) NOT NULL,
        [OrderDate] datetime2 NOT NULL,
        [OrderTotalOriginal] float NOT NULL,
        [OrderTotal] float NOT NULL,
        [PickUpTime] datetime2 NOT NULL,
        [CouponCode] nvarchar(max) NULL,
        [CouponCodeDiscount] float NOT NULL,
        [Status] nvarchar(max) NULL,
        [PaymentStatus] nvarchar(max) NULL,
        [Comments] nvarchar(max) NULL,
        [PickUpName] nvarchar(max) NULL,
        [PhoneNumber] nvarchar(max) NULL,
        [TransactionId] nvarchar(max) NULL,
        CONSTRAINT [PK_OrderHeader] PRIMARY KEY ([Id]),
        CONSTRAINT [FK_OrderHeader_AspNetUsers_UserId] FOREIGN KEY ([UserId]) REFERENCES [AspNetUsers] ([Id]) ON DELETE CASCADE
    );
END;

GO

IF NOT EXISTS(SELECT * FROM [__EFMigrationsHistory] WHERE [MigrationId] = N'20201024014826_AddOrderHeaderAndDetailsToDB')
BEGIN
    CREATE TABLE [OrderDetails] (
        [Id] int NOT NULL IDENTITY,
        [OrderId] int NOT NULL,
        [MenuItemId] int NOT NULL,
        [Count] int NOT NULL,
        [Name] nvarchar(max) NULL,
        [Description] nvarchar(max) NULL,
        [Price] float NOT NULL,
        CONSTRAINT [PK_OrderDetails] PRIMARY KEY ([Id]),
        CONSTRAINT [FK_OrderDetails_MenuItem_MenuItemId] FOREIGN KEY ([MenuItemId]) REFERENCES [MenuItem] ([Id]) ON DELETE CASCADE,
        CONSTRAINT [FK_OrderDetails_OrderHeader_OrderId] FOREIGN KEY ([OrderId]) REFERENCES [OrderHeader] ([Id]) ON DELETE CASCADE
    );
END;

GO

IF NOT EXISTS(SELECT * FROM [__EFMigrationsHistory] WHERE [MigrationId] = N'20201024014826_AddOrderHeaderAndDetailsToDB')
BEGIN
    CREATE INDEX [IX_OrderDetails_MenuItemId] ON [OrderDetails] ([MenuItemId]);
END;

GO

IF NOT EXISTS(SELECT * FROM [__EFMigrationsHistory] WHERE [MigrationId] = N'20201024014826_AddOrderHeaderAndDetailsToDB')
BEGIN
    CREATE INDEX [IX_OrderDetails_OrderId] ON [OrderDetails] ([OrderId]);
END;

GO

IF NOT EXISTS(SELECT * FROM [__EFMigrationsHistory] WHERE [MigrationId] = N'20201024014826_AddOrderHeaderAndDetailsToDB')
BEGIN
    CREATE INDEX [IX_OrderHeader_UserId] ON [OrderHeader] ([UserId]);
END;

GO

IF NOT EXISTS(SELECT * FROM [__EFMigrationsHistory] WHERE [MigrationId] = N'20201024014826_AddOrderHeaderAndDetailsToDB')
BEGIN
    INSERT INTO [__EFMigrationsHistory] ([MigrationId], [ProductVersion])
    VALUES (N'20201024014826_AddOrderHeaderAndDetailsToDB', N'3.1.8');
END;

GO

