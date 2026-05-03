export enum Status {
  Active = 1,
  Inactive = 0,
  Pending = 2,
  Deleted = -1
}

export enum UserRole {
  SuperAdmin = 'SuperAdmin',
  Admin = 'Admin',
  Manager = 'Manager',
  User = 'User',
  Guest = 'Guest'
}

export enum SortDirection {
  Asc = 'asc',
  Desc = 'desc'
}
