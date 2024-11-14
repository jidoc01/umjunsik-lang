module UmFSharp.Core.AST

type ExternalCallKind =
    | ReadInt
    | WriteInt
    | WriteChar

type BinOpKind =
    | Add
    | Sub
    | Mul

with
    member __.OperationFunctor =
        match __ with
        | Add -> (+)
        | Sub -> (-)
        | Mul -> (*)

type Expr =
    | Const of Int
    | ExternalCall of extCallKind: ExternalCallKind * args: Expr list
    | Load of varId: VarId
    | BinOp of operKind: BinOpKind * lhs: Expr * rhs: Expr

type Stmt =
    /// If-not-then statement (동탄).
    | IfNotThen of cond: Expr * Stmt
    /// Goto statement (식).
    | Goto of Expr
    /// Return statement (화이팅!).
    | Ret of Expr
    /// Assignment statement.
    | Def of dstVarId: VarId * srcExpr: Expr
    /// Side effect of an expression.
    /// We ignore the result of the expression.
    | SideEffect of expr: Expr

type Annot = {
    /// The line number of the statement.
    LineNumber: Int
}

type StmtInfo = Annot * Stmt

type Prog = {
    PerLineStmtInfos: Map<Int, StmtInfo>
    Length: Int
}

