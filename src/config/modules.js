export const ROLE_PAGES = {
  Admin: ['dashboard','departments','courses','subjects','semesters','exams','students','faculty','attendance','marks','results','fees','books','categories','announcements','notifications','users','roles','hodAuth','loginHistory','settings'],
  Principal: ['dashboard','departments','courses','subjects','semesters','exams','students','faculty','attendance','marks','results','fees','announcements','notifications','hodAuth','loginHistory'],
  HOD: ['dashboard','students','faculty','attendance','marks','results','announcements','notifications','hodAuth'],
  Faculty: ['dashboard','attendance','marks','announcements','notifications','hodAuth'],
  Student: ['dashboard','attendance','results','announcements','notifications'],
};

export const NAV_ITEMS = [
  { id:'dashboard', label:'Dashboard', category:'Overview' },
  { id:'departments', label:'Departments', category:'Academic Setup' },
  { id:'courses', label:'Courses', category:'Academic Setup' },
  { id:'subjects', label:'Subjects', category:'Academic Setup' },
  { id:'semesters', label:'Semesters', category:'Academic Setup' },
  { id:'exams', label:'Examinations', category:'Academic Setup' },
  { id:'students', label:'Students', category:'People' },
  { id:'faculty', label:'Faculty', category:'People' },
  { id:'attendance', label:'Attendance', category:'Academic Management' },
  { id:'marks', label:'Marks Entry', category:'Academic Management' },
  { id:'results', label:'Results', category:'Academic Management' },
  { id:'fees', label:'Fee Management', category:'Finance' },
  { id:'books', label:'Library Books', category:'Library' },
  { id:'categories', label:'Book Categories', category:'Library' },
  { id:'announcements', label:'Announcements', category:'Communication' },
  { id:'notifications', label:'Notifications', category:'Communication' },
  { id:'users', label:'User Management', category:'Administration' },
  { id:'roles', label:'Roles & Permissions', category:'Administration' },
  { id:'hodAuth', label:'HOD Authorization', category:'Administration' },
  { id:'loginHistory', label:'Login History', category:'Administration' },
  { id:'settings', label:'Settings', category:'System' },
];

const status = { key:'status', label:'Status', type:'select', options:['Active','Inactive'] };
const departmentLookup = { source:'departments', valueKey:'code', labelKeys:['code','name'] };
const studentLookup = { source:'students', valueKey:'id', labelKeys:['rollNo','name'] };
const subjectLookup = { source:'subjects', valueKey:'id', labelKeys:['code','name'] };
const facultyLookup = { source:'faculty', valueKey:'id', labelKeys:['name','department'] };
const categoryLookup = { source:'categories', valueKey:'name', labelKeys:['name','code'] };
const roleLookup = { source:'roles', valueKey:'name', labelKeys:['name'] };

export const MODULES = {
  departments: {
    title:'Departments', resource:'departments', addLabel:'Add Department', writeRoles:['Admin','Principal'],
    columns:[['name','Department'],['code','Code'],['hod','HOD'],['facultyCount','Faculty'],['studentCount','Students'],['status','Status','status']],
    fields:[{key:'name',label:'Name',required:true},{key:'code',label:'Code',required:true},{key:'hod',label:'HOD'},{...status},{key:'description',label:'Description',type:'textarea'}],
  },
 courses: {
  title: "Courses",
  resource: "courses",
  addLabel: "Add Course",

  writeRoles: ["Admin", "Principal"],
  deleteRoles: ["Admin", "Principal"],

  columns: [
    ["name", "Course"],
    ["code", "Code"],
    ["department", "Department"],
    ["duration", "Duration"],
    ["totalSeats", "Seats"],
    ["status", "Status", "status"],
  ],

  fields: [
    {
      key: "name",
      label: "Name",
      required: true,
    },
    {
      key: "code",
      label: "Code",
      required: true,
    },
    {
      key: "department",
      label: "Department",
      type: "lookup",
      lookup: departmentLookup,
      required: true,
    },
    {
      key: "duration",
      label: "Duration",
      placeholder: "4 Years",
    },
    {
      key: "totalSeats",
      label: "Total Seats",
      type: "number",
      default: 60,
    },
    {
      ...status,
    },
  ],
},
  subjects: {
    title:'Subjects', resource:'subjects', addLabel:'Add Subject', writeRoles:['Admin','Principal'],
    columns:[['name','Subject'],['code','Code'],['department','Department'],['semester','Semester'],['credits','Credits'],['type','Type'],['status','Status','status']],
    fields:[{key:'name',label:'Name',required:true},{key:'code',label:'Code',required:true},{key:'department',label:'Department',type:'lookup',lookup:departmentLookup,required:true},{key:'semester',label:'Semester',type:'number',default:1},{key:'credits',label:'Credits',type:'number',default:4},{key:'type',label:'Type',type:'select',options:['Core','Elective','Lab']},{...status}],
  },
  semesters: {
    title:'Semesters', resource:'semesters', addLabel:'Add Semester', writeRoles:['Admin','Principal'],
    columns:[['name','Semester'],['code','Code'],['academicYear','Academic Year'],['startDate','Start Date','date'],['endDate','End Date','date'],['status','Status','status']],
    fields:[{key:'name',label:'Name',required:true},{key:'code',label:'Code',required:true},{key:'academicYear',label:'Academic Year',required:true},{key:'startDate',label:'Start Date',type:'date',required:true},{key:'endDate',label:'End Date',type:'date',required:true},{key:'status',label:'Status',type:'select',options:['Upcoming','Active','Completed']}],
  },
  exams: {
    title:'Examinations', resource:'exams', addLabel:'Add Exam', writeRoles:['Admin','Principal'],
    columns:[['name','Exam'],['code','Code'],['semester','Semester'],['startDate','Start'],['endDate','End'],['type','Type'],['status','Status','status']],
    fields:[{key:'name',label:'Name',required:true},{key:'code',label:'Code',required:true},{key:'semester',label:'Semester',required:true},{key:'startDate',label:'Start Date',type:'date',required:true},{key:'endDate',label:'End Date',type:'date',required:true},{key:'type',label:'Type',type:'select',options:['Mid Sem','End Sem','Internal']},{key:'status',label:'Status',type:'select',options:['Upcoming','Active','Completed']}],
  },
  students: {
    title:'Students', resource:'students', addLabel:'Add Student', writeRoles:['Admin','Principal'], readRoles:['Admin','Principal','HOD'],
    columns:[['name','Student'],['rollNo','Roll No'],['email','Email'],['department','Department'],['semester','Semester'],['phone','Phone'],['feeStatus','Fee'],['status','Status','status']],
    fields:[{key:'name',label:'Name',required:true},{key:'email',label:'Email',type:'email',required:true},{key:'department',label:'Department',type:'lookup',lookup:departmentLookup,required:true},{key:'semester',label:'Semester',type:'number',default:1},{key:'rollNo',label:'Roll Number',required:true},{key:'phone',label:'Phone'},{key:'dob',label:'Date of Birth',type:'date',required:true},{key:'gender',label:'Gender',type:'select',options:['Male','Female','Other']},{key:'feeStatus',label:'Fee Status',type:'select',options:['Paid','Pending','Partial','Overdue']},{...status}],
  },
  faculty: {
    title:'Faculty', resource:'faculty', addLabel:'Add Faculty', writeRoles:['Admin','Principal'],
    columns:[['name','Faculty'],['email','Email'],['department','Department'],['designation','Designation'],['qualification','Qualification'],['phone','Phone'],['joinDate','Joined'],['status','Status','status']],
    fields:[{key:'name',label:'Name',required:true},{key:'email',label:'Email',type:'email',required:true},{key:'department',label:'Department',type:'lookup',lookup:departmentLookup,required:true},{key:'designation',label:'Designation'},{key:'qualification',label:'Qualification'},{key:'phone',label:'Phone'},{key:'joinDate',label:'Join Date',type:'date',required:true},{key:'status',label:'Status',type:'select',options:['Active','Inactive','On Leave']}],
  },
  attendance: {
    title:'Attendance', resource:'attendance', addLabel:'Mark Attendance', writeRoles:['Admin','Principal','HOD','Faculty'], deleteRoles:['Admin','Principal'],
    columns:[['student','Student'],['rollNo','Roll No'],['subject','Subject'],['date','Date'],['status','Status','status'],['markedBy','Marked By']],
    fields:[
      {key:'studentId',label:'Student',type:'lookup',lookup:studentLookup,required:true,match:{itemKey:'rollNo',optionKey:'rollNo'}},
      {key:'subjectId',label:'Subject',type:'lookup',lookup:subjectLookup,required:true,match:{itemKey:'subject',optionKey:'name'}},
      {key:'date',label:'Date',type:'date',required:true},{key:'status',label:'Status',type:'select',options:['Present','Absent','Late']},
      {key:'markedByFacultyId',label:'Faculty',type:'lookup',lookup:facultyLookup,match:{itemKey:'markedBy',optionKey:'name'}},{key:'markedBy',label:'Marked By'},
    ],
  },
  marks: {
    title:'Marks Entry', resource:'marks', addLabel:'Add Marks', writeRoles:['Admin','Principal','HOD','Faculty'], deleteRoles:['Admin','Principal'],
    columns:[['student','Student'],['rollNo','Roll No'],['subject','Subject'],['exam','Exam'],['internal','Internal'],['external','External'],['total','Total'],['maxMarks','Max'],['grade','Grade']],
    fields:[{key:'studentId',label:'Student',type:'lookup',lookup:studentLookup,required:true,match:{itemKey:'rollNo',optionKey:'rollNo'}},{key:'subjectId',label:'Subject',type:'lookup',lookup:subjectLookup,required:true,match:{itemKey:'subject',optionKey:'name'}},{key:'exam',label:'Exam',required:true},{key:'internal',label:'Internal',type:'number',default:0},{key:'external',label:'External',type:'number',default:0},{key:'maxMarks',label:'Maximum Marks',type:'number',default:100},{key:'grade',label:'Grade'}],
  },
  results: {
    title:'Results', resource:'results', addLabel:'Add Result', writeRoles:['Admin','Principal','HOD'], deleteRoles:['Admin','Principal'],
    columns:[['student','Student'],['rollNo','Roll No'],['semester','Semester'],['sgpa','SGPA'],['totalCredits','Credits'],['status','Status','status'],['year','Year']],
    fields:[{key:'studentId',label:'Student',type:'lookup',lookup:studentLookup,required:true,match:{itemKey:'rollNo',optionKey:'rollNo'}},{key:'semester',label:'Semester',required:true},{key:'sgpa',label:'SGPA',type:'number',step:'0.01'},{key:'totalCredits',label:'Total Credits',type:'number'},{key:'status',label:'Status',type:'select',options:['Passed','Failed','Pending']},{key:'year',label:'Year'}],
  },
  fees: {
    title:'Fee Management', resource:'fees', addLabel:'Add Fee', writeRoles:['Admin','Principal'], deleteRoles:['Admin','Principal'],
    columns:[['student','Student'],['rollNo','Roll No'],['semester','Semester'],['totalFee','Total Fee','currency'],['paid','Paid','currency'],['due','Due','currency'],['status','Status','status'],['paidDate','Paid Date'],['mode','Mode']],
    fields:[{key:'studentId',label:'Student',type:'lookup',lookup:studentLookup,required:true,match:{itemKey:'rollNo',optionKey:'rollNo'}},{key:'semester',label:'Semester',required:true},{key:'totalFee',label:'Total Fee',type:'number'},{key:'paid',label:'Paid',type:'number'},{key:'status',label:'Status',type:'select',options:['Paid','Pending','Partial','Overdue']},{key:'paidDate',label:'Paid Date',type:'date'},{key:'mode',label:'Payment Mode',type:'select',options:['Online','Cash','DD','Cheque','']}],
  },
  books: {
    title:'Library Books', resource:'books', addLabel:'Add Book', writeRoles:['Admin','Principal'],
    columns:[['title','Title'],['isbn','ISBN'],['author','Author'],['category','Category'],['copies','Copies'],['available','Available'],['status','Status','status']],
    fields:[{key:'title',label:'Title',required:true},{key:'isbn',label:'ISBN',required:true},{key:'author',label:'Author',required:true},{key:'category',label:'Category',type:'lookup',lookup:categoryLookup,required:true},{key:'copies',label:'Copies',type:'number',default:1},{key:'available',label:'Available',type:'number',default:1},{key:'status',label:'Status',type:'select',options:['Available','All Issued','Inactive']}],
  },
  categories: {
    title:'Book Categories', resource:'categories', addLabel:'Add Category', writeRoles:['Admin','Principal'],
    columns:[['name','Category'],['code','Code'],['bookCount','Books'],['status','Status','status']],
    fields:[{key:'name',label:'Name',required:true},{key:'code',label:'Code',required:true},{...status}],
  },
  announcements: {
    title:'Announcements', resource:'announcements', addLabel:'New Announcement', writeRoles:['Admin','Principal','HOD'],
    columns:[['title','Title'],['target','Target'],['priority','Priority','status'],['postedBy','Posted By'],['date','Date'],['status','Status','status']],
    fields:[{key:'title',label:'Title',required:true},{key:'content',label:'Content',type:'textarea'},{key:'priority',label:'Priority',type:'select',options:['Low','Medium','High']},{key:'target',label:'Target',type:'select',options:['All','Students','Faculty','HOD']},{key:'date',label:'Date',type:'date'},{key:'status',label:'Status',type:'select',options:['Active','Inactive']}],
  },
  users: {
    title:'User Management', resource:'users', addLabel:'Add User', writeRoles:['Admin'],
    columns:[['name','Name'],['email','Email'],['role','Role','role'],['department','Department'],['status','Status','status'],['lastLogin','Last Login','datetime']],
    createFields:[{key:'name',label:'Name',required:true},{key:'email',label:'Email',type:'email',required:true},{key:'password',label:'Password',type:'password',required:true},{key:'role',label:'Role',type:'lookup',lookup:roleLookup,required:true},{key:'department',label:'Department',type:'lookup',lookup:departmentLookup},{key:'status',label:'Status',type:'select',options:['Active','Inactive']},{key:'avatar',label:'Avatar Initials'},{key:'color',label:'Color',default:'#10b981'}],
    editFields:[{key:'name',label:'Name',required:true},{key:'role',label:'Role',type:'lookup',lookup:roleLookup,required:true},{key:'department',label:'Department',type:'lookup',lookup:departmentLookup},{key:'status',label:'Status',type:'select',options:['Active','Inactive']},{key:'avatar',label:'Avatar Initials'},{key:'color',label:'Color'}],
  },
  roles: {
    title:'Roles & Permissions', resource:'roles', addLabel:'Add Role', writeRoles:['Admin'],
    columns:[['name','Role'],['description','Description'],['permissions','Permissions'],['userCount','Users'],['status','Status','status']],
    fields:[{key:'name',label:'Name',required:true},{key:'description',label:'Description',type:'textarea'},{key:'permissions',label:'Permissions',type:'textarea'},{...status}],
  },
  loginHistory: {
    title:'Login History', resource:'loginHistory', readOnly:true,
    columns:[['user','User'],['email','Email'],['ip','IP Address'],['device','Device'],['time','Time','datetime'],['status','Status','status']],
  },
};
